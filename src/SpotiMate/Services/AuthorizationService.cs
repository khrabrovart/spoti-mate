using SpotiMate.Cli;
using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Responses;

namespace SpotiMate.Services;

public class AuthorizationService
{
    private readonly SpotifyAuthorizationApi _spotifyAuthorizationApi = new();

    private TokenResponse _token;

    public async Task<string> GetAccessToken(string clientId, string clientSecret, string refreshToken)
    {
        _token ??= await Authorize(clientId, clientSecret, refreshToken);
        return _token?.AccessToken;
    }

    private async Task<TokenResponse> Authorize(string clientId, string clientSecret, string refreshToken)
    {
        CliPrint.PrintInfo("Authorizing with Spotify...");
        
        var token = await _spotifyAuthorizationApi.RefreshAccessToken(clientId, clientSecret, refreshToken);

        if (token == null)
        {
            CliPrint.PrintError("Authorization failed. Please check your credentials and try again.");
            return null;
        }

        CliPrint.PrintSuccess("Authorization successful!");
        return token;
    }
}
