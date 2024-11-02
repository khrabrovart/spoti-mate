using SpotiMate.Spotify.Models;
using SpotiMate.Spotify.Providers;

namespace SpotiMate.Spotify.Apis;

public class SpotifySearchApi : SpotifyApiBase, ISpotifySearchApi
{
    public SpotifySearchApi(ISpotifyAuthProvider authProvider) : base(authProvider, "search")
    {
    }

    public async Task<ApiResponse<SearchResults>> SearchTracks(string query, int offset, int limit)
    {
        FieldValidator.Int32(nameof(offset), offset, min: 0, max: 1000);
        FieldValidator.Int32(nameof(limit), limit, min: 0, max: 50);

        var queryParams = new Dictionary<string, string>
        {
            { "q", query },
            { "type", "track" },
            { "offset", offset.ToString() },
            { "limit", limit.ToString() }
        };

        return await MakeRequest<SearchResults>(
            HttpMethod.Get,
            queryParams: queryParams);
    }
}
