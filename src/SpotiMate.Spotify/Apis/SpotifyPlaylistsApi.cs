using SpotiMate.Spotify.Extensions;
using SpotiMate.Spotify.Models;
using SpotiMate.Spotify.Providers;

namespace SpotiMate.Spotify.Apis;

public interface ISpotifyPlaylistsApi
{
    Task<ApiResponse<Page<PlaylistTrackObject>>> GetPlaylistTracks(string playlistId, int offset, int limit);
    Task<ApiResponse> AddTracksToPlaylist(string playlistId, string[] trackIds);
    Task<ApiResponse<Playlist>> CreatePlaylist(string userId, string playlistName);
    Task<ApiResponse> RemoveTracksFromPlaylist(string playlistId, string[] trackIds);
}

public class SpotifyPlaylistsApi : SpotifyApiBase, ISpotifyPlaylistsApi
{
    public SpotifyPlaylistsApi(ISpotifyAuthProvider authProvider) : base(authProvider, "playlists")
    {
    }

    public async Task<ApiResponse<Page<PlaylistTrackObject>>> GetPlaylistTracks(string playlistId, int offset, int limit)
    {
        FieldValidator.Int(nameof(offset), offset, min: 0);
        FieldValidator.Int(nameof(limit), limit, min: 0, max: 50);

        var queryParams = new Dictionary<string, string>
        {
            { "offset", offset.ToString() },
            { "limit", limit.ToString() }
        };

        return await MakeRequest<Page<PlaylistTrackObject>>(
            HttpMethod.Get,
            segment: $"{playlistId}/tracks",
            queryParams: queryParams);
    }

    public async Task<ApiResponse<Playlist>> CreatePlaylist(string userId, string playlistName)
    {
        var body = new
        {
            name = playlistName
        };

        return await MakeRequest<Playlist>(
            HttpMethod.Post,
            segment: $"users/{userId}/playlists",
            body: body);
    }

    public async Task<ApiResponse> AddTracksToPlaylist(string playlistId, string[] trackIds)
    {
        FieldValidator.Length(nameof(trackIds), trackIds, min: 0, max: 100);

        var body = new
        {
            uris = trackIds.Select(id => id.ToSpotifyTrackUri())
        };

        return await MakeRequest(
            HttpMethod.Post,
            segment: $"{playlistId}/tracks",
            body: body);
    }

    public async Task<ApiResponse> RemoveTracksFromPlaylist(string playlistId, string[] trackIds)
    {
        FieldValidator.Length(nameof(trackIds), trackIds, min: 0, max: 100);

        var body = new
        {
            tracks = trackIds.Select(id => new
            {
                uri = id.ToSpotifyTrackUri()
            })
        };

        return await MakeRequest(
            HttpMethod.Delete,
            segment: $"{playlistId}/tracks",
            body: body);
    }
}
