using Flurl;
using Flurl.Http;
using SpotiMate.Spotify.Extensions;
using SpotiMate.Spotify.Models;
using SpotiMate.Spotify.Providers;

namespace SpotiMate.Spotify.Apis;

public abstract class SpotifyApiBase
{
    private const int RetryCount = 3;

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
        ApiResponse response = null;

        for (var i = 0; i < RetryCount; i++)
        {
            var rawResponse = await MakeInternalRequest(verb, segment, body, queryParams);
            response = await rawResponse.ToApiResponse();

            if (!response.IsError)
            {
                return response;
            }

            if (response.RetryAfter.HasValue)
            {
                await Task.Delay(TimeSpan.FromSeconds(response.RetryAfter.Value));
            }
        }

        return response;
    }

    protected async Task<ApiResponse<TResponse>> MakeRequest<TResponse>(
        HttpMethod verb,
        string segment = null,
        object body = null,
        Dictionary<string, string> queryParams = null)
    {
        ApiResponse<TResponse> response = null;

        for (var i = 0; i < RetryCount; i++)
        {
            var rawResponse = await MakeInternalRequest(verb, segment, body, queryParams);
            response = await rawResponse.ToApiResponse<TResponse>();

            if (!response.IsError)
            {
                return response;
            }

            if (response.RetryAfter.HasValue)
            {
                await Task.Delay(TimeSpan.FromSeconds(response.RetryAfter.Value));
            }
        }

        return response;
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
