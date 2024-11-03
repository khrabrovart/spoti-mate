using Flurl;
using Flurl.Http;
using SpotiMate.Spotify.Models;

namespace SpotiMate.Spotify.Providers;

public class SpotifyAuthProvider : ISpotifyAuthProvider
{
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _refreshToken;

    private string _accessToken;

    public SpotifyAuthProvider(string clientId, string clientSecret, string refreshToken)
    {
        _clientId = clientId;
        _clientSecret = clientSecret;
        _refreshToken = refreshToken;
    }

    public async Task<string> GetAccessToken()
    {
        _accessToken ??= await RefreshAccessToken();

        if (_accessToken == null)
        {
            throw new Exception("Failed to authorize with Spotify. Please check your credentials and try again.");
        }

        return _accessToken;
    }

    private async Task<string> RefreshAccessToken()
    {
        try
        {
            var response = await SpotifyEndpoints.Accounts
                .AppendPathSegment("api/token")
                .WithBasicAuth(_clientId, _clientSecret)
                .PostUrlEncodedAsync(new
                {
                    refresh_token = _refreshToken,
                    grant_type = "refresh_token"
                })
                .ReceiveJson<AuthorizationData>();

            return response.AccessToken;
        }
        catch (FlurlHttpException)
        {
            return null;
        }
    }
}
