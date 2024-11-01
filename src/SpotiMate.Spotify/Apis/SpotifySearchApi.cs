using Flurl;
using Flurl.Http;
using SpotiMate.Spotify.Authorization;
using SpotiMate.Spotify.Responses;

namespace SpotiMate.Spotify.Apis;

public class SpotifySearchApi : ISpotifySearchApi
{
    private readonly string _accessToken;

    public SpotifySearchApi(SpotifyAccessToken accessToken)
    {
        _accessToken = accessToken;
    }

    private IFlurlRequest CreateApiRequest(string segment, Dictionary<string, string> queryParams = null)
    {
        return SpotifyEndpoints.Api
            .AppendPathSegment(segment)
            .SetQueryParams(queryParams)
            .WithOAuthBearerToken(_accessToken);
    }

    public async Task<SpotifySearchResponse> SearchTracks(string query, int limit = 50, int offset = 0)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "q", query },
            { "type", "track" },
            { "limit", limit.ToString() },
            { "offset", offset.ToString() }
        };

        return await CreateApiRequest("search", queryParams).GetJsonAsync<SpotifySearchResponse>();
    }
}
