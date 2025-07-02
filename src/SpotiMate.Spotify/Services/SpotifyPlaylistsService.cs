using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Models;

namespace SpotiMate.Spotify.Services;

public interface ISpotifyPlaylistsService : ISpotifyService<ISpotifyPlaylistsApi>
{
    Task<PlaylistTrackObject[]> GetPlaylistTracks(string playlistId);
    Task AddTracksToPlaylist(string playlistId, string[] trackIds);
    Task<string> CreatePlaylist(string userId, string playlistName);
    Task RemoveTracksFromPlaylist(string playlistId, string[] trackIds);
}

public class SpotifyPlaylistsService : ISpotifyPlaylistsService
{
    public SpotifyPlaylistsService(ISpotifyPlaylistsApi spotifyPlaylistsApi)
    {
        Api = spotifyPlaylistsApi;
    }

    public ISpotifyPlaylistsApi Api { get; }

    public async Task<PlaylistTrackObject[]> GetPlaylistTracks(string playlistId)
    {
        var tracks = new List<PlaylistTrackObject>();

        const int limit = 50;
        var offset = 0;

        while (true)
        {
            var page = await Api.GetPlaylistTracks(playlistId, offset, limit);

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

    public async Task AddTracksToPlaylist(string playlistId, string[] trackIds)
    {
        const int chunkSize = 50;
        var chunks = trackIds.Chunk(chunkSize);

        foreach (var chunk in chunks)
        {
            var result = await Api.AddTracksToPlaylist(playlistId, chunk);

            if (result.IsError)
            {
                throw new Exception($"Failed to add tracks to playlist: {result.Error}");
            }
        }
    }

    public async Task<string> CreatePlaylist(string userId, string playlistName)
    {
        var result = await Api.CreatePlaylist(userId, playlistName);

        if (result.IsError)
        {
            throw new Exception($"Failed to create playlist: {result.Error}");
        }

        return result.Data.Id;
    }

    public async Task RemoveTracksFromPlaylist(string playlistId, string[] trackIds)
    {
        const int chunkSize = 50;
        var chunks = trackIds.Chunk(chunkSize);

        foreach (var chunk in chunks)
        {
            var result = await Api.RemoveTracksFromPlaylist(playlistId, chunk);

            if (result.IsError)
            {
                throw new Exception($"Failed to remove tracks from playlist: {result.Error}");
            }
        }
    }
}
