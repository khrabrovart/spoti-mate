using SpotiMate.Spotify.Models;

namespace SpotiMate.Spotify.Apis;

public interface ISpotifyUsersApi
{
    Task<ApiResponse<Playlist>> CreatePlaylist(string userId, string playlistName);
}
