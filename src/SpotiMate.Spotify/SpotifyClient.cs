using Flurl;
using Flurl.Http;
using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Authorization;
using SpotiMate.Spotify.Extensions;
using SpotiMate.Spotify.Objects;
using SpotiMate.Spotify.Responses;

namespace SpotiMate.Spotify;

public class SpotifyClient
{
    private readonly SpotifyParallelProcessor _spotifyParallelProcessor = new();
    private readonly SpotifyAuthorizer _authorizer = new();
    
    private string _accessToken;
    
    public async Task Authorize(string clientId, string clientSecret, string refreshToken)
    {
        //_accessToken = await _authorizationApi.GetAccessToken(clientId, clientSecret, refreshToken);
    }

    private IFlurlRequest CreateApiRequest(string segment)
    {
        return SpotifyEndpoints.Api
            .AppendPathSegment(segment)
            .WithOAuthBearerToken(_accessToken);
    }
    
    public async Task<SavedTrackObject[]> GetSavedTracks()
    {
        return await _spotifyParallelProcessor.GetAll<GetSavedTracksResponse, SavedTrackObject>(
            () => CreateApiRequest("me/tracks"),
            pageSize: 50);
    }
    
    public async Task<IReadOnlyCollection<PlaylistTrackObject>> GetPlaylistTracks(string playlistId)
    {
        return await _spotifyParallelProcessor.GetAll<GetPlaylistItemsResponse, PlaylistTrackObject>(
            () => CreateApiRequest($"playlists/{playlistId}/tracks"),
            pageSize: 50);
    }
    
    public async Task<bool> AddTracksToPlaylist(string playlistId, IEnumerable<string> trackIds)
    {
        return await _spotifyParallelProcessor.ProcessAll(
            chunk => CreateApiRequest($"playlists/{playlistId}/tracks")
                .PostJsonAsync(new
                {
                    uris = chunk.Select(id => id.ToSpotifyTrackUri())
                }),
            trackIds,
            chunkSize: 100);
    }
    
    public async Task<bool> RemoveTracksFromPlaylist(string playlistId, IEnumerable<string> trackIds)
    {
        return await _spotifyParallelProcessor.ProcessAll(
            chunk => CreateApiRequest($"playlists/{playlistId}/tracks")
                .SendJsonAsync(HttpMethod.Delete, new
                {
                    tracks = chunk.Select(id => new { uri = id.ToSpotifyTrackUri() })
                }),
            trackIds,
            chunkSize: 100);
    }
    
    public async Task<bool> FollowArtists(IEnumerable<string> artistIds)
    {
        return await _spotifyParallelProcessor.ProcessAll(
            chunk => CreateApiRequest("me/following")
                .SetQueryParams(new { type = "artist" })
                .PutJsonAsync(new
                {
                    ids = chunk
                }),
            artistIds,
            chunkSize: 50);
    }
}
