using Flurl;
using Flurl.Http;
using SpotiMate.Cli;
using SpotiMate.Spotify.Responses;

namespace SpotiMate.Spotify;

public class SpotifyAuthorizer
{
    private TokenResponse _token;
    
    public async Task<string> GetAccessToken(string clientId, string clientSecret, string refreshToken)
    {
        _token ??= await Authorize(clientId, clientSecret, refreshToken);
        return _token?.AccessToken;
    }
    
    private static async Task<TokenResponse> Authorize(string clientId, string clientSecret, string refreshToken)
    {
        try
        {
            CliPrint.PrintInfo("Authorizing with Spotify...");
            var token = await RefreshAccessToken(clientId, clientSecret, refreshToken);
            CliPrint.PrintSuccess("Authorization successful!");
            
            return token;
        }
        catch (FlurlHttpException)
        {
            CliPrint.PrintError("Authorization failed. Please check your credentials and try again.");
            
            return null;
        }
    }
    
    private static async Task<TokenResponse> RefreshAccessToken(
        string clientId,
        string clientSecret,
        string refreshToken)
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
}