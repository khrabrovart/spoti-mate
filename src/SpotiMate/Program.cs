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
        throw new SpotifyClientException("This is a test exception.");
        var spotify = new SpotifyClient();
        await spotify.Authorize(options.ClientId, options.ClientSecret, options.RefreshToken);
        
        CliPrint.PrintInfo("Loading saved tracks...");
        var savedTracks = await spotify.GetSavedTracks();
        CliPrint.PrintSuccess($"Found {savedTracks.Count} saved tracks.");
            
        await new FavoritesSynchronizationService().SynchronizeFavorites(
            spotify, 
            savedTracks,
            options.FavoritesPlaylistId);
        
        CliPrint.PrintInfo("Favorites synchronized.");
        CliPrint.PrintSuccess("Done.");
    }
}