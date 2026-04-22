using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpotiMate.Cli;
using SpotiMate.Handlers;
using SpotiMate.Services;
using SpotiMate.Spotify;

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
            .AddSingleton<ISpotifyClient>(_ => new SpotifyClient(options.ClientId, options.ClientSecret, options.RefreshToken))
            .AddTransient<ICommandHandler, CommandHandler>()
            .AddTransient<IDuplicateService, DuplicateService>()
            .AddTransient<IBlendService, BlendService>()
            .AddTransient<IArtistService, ArtistService>();
    }
}
