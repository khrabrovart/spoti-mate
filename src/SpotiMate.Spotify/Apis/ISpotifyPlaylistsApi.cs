using SpotiMate.Spotify.Models;

namespace SpotiMate.Spotify.Apis;

public interface ISpotifyPlaylistsApi
{
    Task<ApiResponse<Page<PlaylistTrackObject>>> GetPlaylistTracks(string playlistId, int offset, int limit);

    Task<ApiResponse> AddTracksToPlaylist(string playlistId, string[] trackIds);

    Task<ApiResponse> RemovePlaylistTracks(string playlistId, string[] trackIds);
}
