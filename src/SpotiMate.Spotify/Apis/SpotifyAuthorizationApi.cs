using Flurl;
using Flurl.Http;
using SpotiMate.Spotify.Responses;

namespace SpotiMate.Spotify.Apis;

public class SpotifyAuthorizationApi
{
    public async Task<TokenResponse> RefreshAccessToken(string clientId, string clientSecret, string refreshToken)
    {
        try
        {
            return await SpotifyEndpoints.Accounts
                .AppendPathSegment("api/token")
                .WithBasicAuth(clientId, clientSecret)
                .PostUrlEncodedAsync(new
                {
                    refresh_token = refreshToken,
                    grant_type = "refresh_token"
                })
                .ReceiveJson<TokenResponse>();
        }
        catch (FlurlHttpException)
        {
            return null;
        }
    }
}
