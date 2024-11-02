using SpotiMate.Spotify.Models;

namespace SpotiMate.Spotify.Apis;

public interface ISpotifySearchApi
{
    Task<ApiResponse<SearchResults>> SearchTracks(string query, int offset, int limit);
}
