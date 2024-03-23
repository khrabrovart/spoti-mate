using CommandLine;
using SpotiMate.Cli;
using SpotiMate.Spotify;

namespace SpotiMate;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            await Parser.Default.ParseArguments<CliOptions>(args).WithParsedAsync(Run);
        }
        catch (Exception ex)
        {
            CliPrint.PrintError(ex.Message);
            return 1;
        }
        
        return 0;
    }
    
    private static async Task Run(CliOptions options)
    {
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