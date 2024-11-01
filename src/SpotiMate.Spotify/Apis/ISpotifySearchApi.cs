using SpotiMate.Spotify.Responses;

namespace SpotiMate.Spotify.Apis;

public interface ISpotifySearchApi
{
    Task<SpotifySearchResponse> SearchTracks(string query, int limit = 50, int offset = 0);
}
