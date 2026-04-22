using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Models.Responses;

namespace SpotiMate.Spotify.Services;

public interface ISpotifyPlaylistsService : ISpotifyService<ISpotifyPlaylistsApi>
{
    Task<PlaylistItemObject[]> GetPlaylistItems(string playlistId);
    Task AddItemsToPlaylist(string playlistId, string[] itemIds);
    Task RemoveItemsFromPlaylist(string playlistId, string[] itemIds);
}

public class SpotifyPlaylistsService : ISpotifyPlaylistsService
{
    public SpotifyPlaylistsService(ISpotifyPlaylistsApi spotifyPlaylistsApi)
    {
        Api = spotifyPlaylistsApi;
    }

    public ISpotifyPlaylistsApi Api { get; }

    public async Task<PlaylistItemObject[]> GetPlaylistItems(string playlistId)
    {
        var tracks = new List<PlaylistItemObject>();

        const int limit = 50;
        var offset = 0;

        while (true)
        {
            var page = await Api.GetPlaylistItems(playlistId, offset, limit);

            if (page.IsError)
            {
                throw new Exception($"Failed to get playlist tracks: {page.Error}");
            }

            tracks.AddRange(page.Data.Items);

            if (tracks.Count >= page.Data.Total)
            {
                return tracks.ToArray();
            }

            offset += limit;
        }
    }

    public async Task AddItemsToPlaylist(string playlistId, string[] itemIds)
    {
        const int chunkSize = 100;
        var chunks = itemIds.Chunk(chunkSize);

        foreach (var chunk in chunks)
        {
            var result = await Api.AddItemsToPlaylist(playlistId, chunk);

            if (result.IsError)
            {
                throw new Exception($"Failed to add tracks to playlist: {result.Error}");
            }
        }
    }

    public async Task RemoveItemsFromPlaylist(string playlistId, string[] itemIds)
    {
        const int chunkSize = 100;
        var chunks = itemIds.Chunk(chunkSize);

        foreach (var chunk in chunks)
        {
            var result = await Api.RemoveItemsFromPlaylist(playlistId, chunk);

            if (result.IsError)
            {
                throw new Exception($"Failed to remove tracks from playlist: {result.Error}");
            }
        }
    }
}
