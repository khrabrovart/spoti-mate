using Flurl;
using Flurl.Http;
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
        _accessToken = await _authorizer.GetAccessToken(clientId, clientSecret, refreshToken);
    }

    private IFlurlRequest MakeRequest(string segment)
    {
        return SpotifyEndpoints.Api
            .AppendPathSegment(segment)
            .WithOAuthBearerToken(_accessToken);
    }

    public async Task<IReadOnlyCollection<SavedTrackObject>> GetSavedTracks()
    {
        return await _spotifyParallelProcessor.GetAll<GetSavedTracksResponse, SavedTrackObject>(
            MakeRequest("me/tracks"),
            limit: 50, 
            r => r.Items);
    }
    
    public async Task<IReadOnlyCollection<PlaylistTrackObject>> GetPlaylistTracks(string playlistId)
    {
        return await _spotifyParallelProcessor.GetAll<GetPlaylistItemsResponse, PlaylistTrackObject>(
            MakeRequest($"playlists/{playlistId}/tracks"),
            limit: 50, 
            r => r.Items);
    }
    
    public async Task<bool> AddTracksToPlaylist(string playlistId, string[] trackIds)
    {
        return await _spotifyParallelProcessor.ProcessAll(
            trackIds,
            chunkSize: 100,
            async chunk =>
            {
                var response = await MakeRequest($"playlists/{playlistId}/tracks")
                    .PostJsonAsync(new { uris = chunk.Select(id => $"spotify:track:{id}") });

                return response.StatusCode == 201;
            });
    }
    
    public async Task<bool> RemoveTracksFromPlaylist(string playlistId, string[] trackIds)
    {
        return await _spotifyParallelProcessor.ProcessAll(
            trackIds,
            chunkSize: 100,
            async chunk =>
            {
                var response = await MakeRequest($"playlists/{playlistId}/tracks")
                    .SendJsonAsync(HttpMethod.Delete, new
                    {
                        tracks = chunk.Select(id => new { uri = $"spotify:track:{id}" })
                    });

                return response.StatusCode == 200;
            });
    }
}