using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpotiMate.Cli;
using SpotiMate.Handlers;
using SpotiMate.Services;
using SpotiMate.Spotify.Authorization;

namespace SpotiMate;

public class Bootstrapper
{
    public static IServiceProvider Bootstrap(CliOptions options)
    {
        var host = Host
            .CreateDefaultBuilder()
            .ConfigureServices((_, services) => ConfigureServices(services, options))
            .Build();

        return host.Services;
    }

    private static void ConfigureServices(IServiceCollection services, CliOptions options)
    {
        services
            .AddSingleton(async () => await GetSpotifyAccessToken(options))
            .AddTransient<ICommandHandler, CommandHandler>()
            .AddTransient<ISearchService, SearchService>();
    }

    private static async Task<SpotifyAccessToken> GetSpotifyAccessToken(CliOptions options)
    {
        var accessToken = await SpotifyAuthorizer.Authorize(options.ClientId, options.ClientSecret, options.RefreshToken);

        if (accessToken == null)
        {
            throw new Exception("Failed to authorize with Spotify. Please check your credentials and try again.");
        }

        return accessToken;
    }
}
