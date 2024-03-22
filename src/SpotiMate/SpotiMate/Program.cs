using CommandLine;
using Flurl;
using Flurl.Http;
using SpotiMate.Cli;

namespace SpotiMate;

public class Program
{
    private const string SpotifyAccounts = "https://accounts.spotify.com";

    public static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments<CliOptions>(args).WithParsedAsync(Run);
    }
    
    private static async Task Run(CliOptions options)
    {
        var token = await GetAccessToken(options.ClientId, options.ClientSecret, options.RefreshToken);
    }
    
    private static async Task<SpotifyTokenResponse> GetAccessToken(
        string clientId,
        string clientSecret, 
        string refreshToken)
    {
        return await SpotifyAccounts
            .AppendPathSegment("api/token")
            .WithBasicAuth(clientId, clientSecret)
            .PostUrlEncodedAsync(new
            {
                refresh_token = refreshToken,
                grant_type = "refresh_token"
            })
            .ReceiveJson<SpotifyTokenResponse>();
    }
}