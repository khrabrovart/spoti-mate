namespace SpotiMate.Services;

public interface IPlaylistService
{
    Task<string> CreatePlaylist(string playlistName);

    Task<bool> AddTracksToPlaylist(string playlistId, string[] trackIds);
}
