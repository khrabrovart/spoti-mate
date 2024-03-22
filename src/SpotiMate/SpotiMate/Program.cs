using CommandLine;
using SpotiMate.Cli;
using SpotiMate.Spotify;

namespace SpotiMate;

public class Program
{
    public static async Task Main(string[] args)
    {
        await Parser.Default.ParseArguments<CliOptions>(args).WithParsedAsync(Run);
    }
    
    private static async Task Run(CliOptions options)
    {
        var spotify = new SpotifyClient();
        await spotify.Authorize(options.ClientId, options.ClientSecret, options.RefreshToken);
        
        await new FavoritesSynchronizationService().SynchronizeFavorites(spotify, options.FavoritesPlaylistId);
    }
}