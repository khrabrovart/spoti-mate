using SpotiMate.Spotify.Extensions;
using SpotiMate.Spotify.Models.Responses;
using SpotiMate.Spotify.Providers;

namespace SpotiMate.Spotify.Apis;

public interface ISpotifyPlaylistsApi
{
    Task<ApiResponse<Page<PlaylistItemObject>>> GetPlaylistItems(string playlistId, int offset, int limit);
    Task<ApiResponse> AddItemsToPlaylist(string playlistId, string[] itemIds);
    Task<ApiResponse> RemoveItemsFromPlaylist(string playlistId, string[] itemIds);
}

public class SpotifyPlaylistsApi : SpotifyApiBase, ISpotifyPlaylistsApi
{
    public SpotifyPlaylistsApi(ISpotifyAuthProvider authProvider) : base(authProvider, "playlists")
    {
    }

    public async Task<ApiResponse<Page<PlaylistItemObject>>> GetPlaylistItems(string playlistId, int offset, int limit)
    {
        FieldValidator.Int(nameof(offset), offset, min: 0);
        FieldValidator.Int(nameof(limit), limit, min: 0, max: 50);

        var queryParams = new Dictionary<string, string>
        {
            { "offset", offset.ToString() },
            { "limit", limit.ToString() }
        };

        return await MakeRequest<Page<PlaylistItemObject>>(
            HttpMethod.Get,
            segment: $"{playlistId}/items",
            queryParams: queryParams);
    }

    public async Task<ApiResponse> AddItemsToPlaylist(string playlistId, string[] itemIds)
    {
        FieldValidator.Length(nameof(itemIds), itemIds, min: 0, max: 100);

        var body = new
        {
            uris = itemIds.Select(id => id.ToSpotifyTrackUri())
        };

        return await MakeRequest(
            HttpMethod.Post,
            segment: $"{playlistId}/items",
            body: body);
    }

    public async Task<ApiResponse> RemoveItemsFromPlaylist(string playlistId, string[] itemIds)
    {
        FieldValidator.Length(nameof(itemIds), itemIds, min: 0, max: 100);

        var body = new
        {
            items = itemIds.Select(id => new
            {
                uri = id.ToSpotifyTrackUri()
            })
        };

        return await MakeRequest(
            HttpMethod.Delete,
            segment: $"{playlistId}/items",
            body: body);
    }
}
