using System.Net;
using Flurl.Http;
using SpotiMate.Spotify.Models;

namespace SpotiMate.Spotify.Extensions;

static internal class ResponseExtensions
{
    public static async Task<ApiResponse> ToApiResponse(this IFlurlResponse response)
    {
        var apiResponse = new ApiResponse();
        await ResolveCommonFields(response, apiResponse);

        return apiResponse;
    }

    public static async Task<ApiResponse<TResponse>> ToApiResponse<TResponse>(this IFlurlResponse response)
    {
        var apiResponse = new ApiResponse<TResponse>();
        await ResolveCommonFields(response, apiResponse);
        apiResponse.Data = !apiResponse.IsError ? await response.GetJsonAsync<TResponse>() : default;

        return apiResponse;
    }

    private static async Task ResolveCommonFields(IFlurlResponse response, ApiResponse apiResponse)
    {
        apiResponse.StatusCode = (HttpStatusCode)response.StatusCode;
        apiResponse.IsError = response.StatusCode is < 200 or > 299;
        apiResponse.Error = apiResponse.IsError ? await response.GetStringAsync() : default;
        apiResponse.RetryAfter = ResolveRetryAfter(response);
    }

    private static int? ResolveRetryAfter(IFlurlResponse response)
    {
        if (response.Headers.TryGetFirst("Retry-After", out var headerValue))
        {
            return int.TryParse(headerValue, out var parsedValue) ? parsedValue : default;
        }

        return default;
    }
}
