using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpotiMate.Cli;
using SpotiMate.Handlers;
using SpotiMate.OpenAI.Extensions;
using SpotiMate.Services;
using SpotiMate.Spotify.Extensions;

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

            .AddTransient<ISavedTracksService, SavedTracksService>()
            .AddTransient<IDuplicatesService, DuplicatesService>()
            .AddTransient<IArtistsService, ArtistsService>()
            .AddTransient<ISearchService, SearchService>()

            .AddSpotify(options.ClientId, options.ClientSecret, options.RefreshToken)
            .AddOpenAI();
    }
}
