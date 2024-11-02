using Flurl;
using Flurl.Http;
using SpotiMate.Spotify.Extensions;
using SpotiMate.Spotify.Models;
using SpotiMate.Spotify.Providers;

namespace SpotiMate.Spotify.Apis;

public abstract class SpotifyApiBase
{
    private readonly ISpotifyAuthProvider _authProvider;
    private readonly string _apiSegment;

    protected SpotifyApiBase(ISpotifyAuthProvider authProvider, string apiSegment)
    {
        _authProvider = authProvider;
        _apiSegment = apiSegment;
    }

    protected async Task<ApiResponse> MakeRequest(
        HttpMethod verb,
        string segment = null,
        object body = null,
        Dictionary<string, string> queryParams = null)
    {
        var response = await MakeInternalRequest(verb, segment, body, queryParams);
        return await response.ToApiResponse();
    }

    protected async Task<ApiResponse<TResponse>> MakeRequest<TResponse>(
        HttpMethod verb,
        string segment = null,
        object body = null,
        Dictionary<string, string> queryParams = null)
    {
        var response = await MakeInternalRequest(verb, segment, body, queryParams);
        return await response.ToApiResponse<TResponse>();
    }

    private async Task<IFlurlResponse> MakeInternalRequest(
        HttpMethod verb,
        string segment,
        object body,
        Dictionary<string, string> queryParams)
    {
        var url = Url.Combine(SpotifyEndpoints.Api, _apiSegment, segment);
        var accessToken = await _authProvider.GetAccessToken();

        var request = url
            .WithOAuthBearerToken(accessToken)
            .SetQueryParams(queryParams)
            .AllowAnyHttpStatus();

        return body == null
            ? await request.SendAsync(verb)
            : await request.SendJsonAsync(verb, body);
    }
}
