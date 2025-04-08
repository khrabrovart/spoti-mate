using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Models;

namespace SpotiMate.Spotify.Services;

public interface ISpotifyPlaylistsService
{
    Task<PlaylistTrackObject[]> GetPlaylistTracks(string playlistId);
    Task AddTracksToPlaylist(string playlistId, string[] trackIds);
    Task RemoveTracksFromPlaylist(string playlistId, string[] trackIds);
}

public class SpotifyPlaylistsService : ISpotifyPlaylistsService
{
    private readonly ISpotifyPlaylistsApi _spotifyPlaylistsApi;

    public SpotifyPlaylistsService(ISpotifyPlaylistsApi spotifyPlaylistsApi)
    {
        _spotifyPlaylistsApi = spotifyPlaylistsApi;
    }

    public async Task<PlaylistTrackObject[]> GetPlaylistTracks(string playlistId)
    {
        var tracks = new List<PlaylistTrackObject>();

        const int limit = 50;
        var offset = 0;

        while (true)
        {
            var page = await _spotifyPlaylistsApi.GetPlaylistTracks(playlistId, offset, limit);

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
            var result = await _spotifyPlaylistsApi.AddTracksToPlaylist(playlistId, chunk);

            if (result.IsError)
            {
                throw new Exception($"Failed to add tracks to playlist: {result.Error}");
            }
        }
    }

    public async Task RemoveTracksFromPlaylist(string playlistId, string[] trackIds)
    {
        const int chunkSize = 50;
        var chunks = trackIds.Chunk(chunkSize);

        foreach (var chunk in chunks)
        {
            var result = await _spotifyPlaylistsApi.RemoveTracksFromPlaylist(playlistId, chunk);

            if (result.IsError)
            {
                throw new Exception($"Failed to remove tracks from playlist: {result.Error}");
            }
        }
    }
}
