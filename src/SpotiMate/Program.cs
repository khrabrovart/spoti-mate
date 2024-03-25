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
            var options = Parser.Default.ParseArguments<CliOptions>(args).Value;
            
            if (options == null)
            {
                CliPrint.PrintError("Invalid arguments.");
                return 1;
            }
            
            var result = await Run(options) ? 0 : 1;
            
            CliPrint.PrintSuccess("Done.", writeLine: false);
            
            return result;
        }
        catch (Exception ex)
        {
            CliPrint.PrintError(ex.Message);
            return 1;
        }
    }
    
    private static async Task<bool> Run(CliOptions options)
    {
        var spotify = new SpotifyClient();
        await spotify.Authorize(options.ClientId, options.ClientSecret, options.RefreshToken);
        
        CliPrint.PrintInfo("Loading saved tracks...");
        var savedTracks = await spotify.GetSavedTracks();
        CliPrint.PrintSuccess($"Found {savedTracks.Count} saved tracks.");

        var success = true;
            
        success &= await new FavoritesSynchronizationService().SynchronizeFavorites(
            spotify, 
            savedTracks,
            options.FavoritesPlaylistId);
        
        CliPrint.PrintSuccess("Favorites synchronized.");
        
        return success;
    }
}