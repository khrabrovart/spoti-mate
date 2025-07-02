using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpotiMate.Cli;
using SpotiMate.Handlers;
using SpotiMate.OpenAI.Extensions;
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
            .AddTransient<ICommandHandler, CommandHandler>()

            .AddSingleton<ISpotifyClient>(_ => new SpotifyClient(options.ClientId, options.ClientSecret, options.RefreshToken))

            .AddTransient<IDuplicateService, DuplicateService>()
            .AddTransient<IArtistService, ArtistService>()
            .AddTransient<IBlendService, BlendService>()

            .AddOpenAI();
    }
}
