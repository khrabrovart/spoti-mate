using CommandLine;
using SpotiMate.Cli;
using SpotiMate.Services;
using SpotiMate.Spotify;

namespace SpotiMate;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var options = Parser.Default
                .ParseArguments<SynchronizeFavoritesOptions, SynchronizeArtistsOptions>(args)
                .Value;
            
            if (options is not CliOptions parsedOptions)
            {
                CliPrint.PrintError("Invalid arguments.");
                return 1;
            }

            var spotify = new SpotifyClient();
            await spotify.Authorize(parsedOptions.ClientId, parsedOptions.ClientSecret, parsedOptions.RefreshToken);
            
            return options switch
            {
                SynchronizeFavoritesOptions typedOptions => await SynchronizeFavorites(spotify, typedOptions),
                SynchronizeArtistsOptions typedOptions => await SynchronizeArtists(spotify, typedOptions),
                _ => 1
            };
        }
        catch (Exception ex)
        {
            CliPrint.PrintError(ex.Message);
            return 1;
        }
    }
    
    private static async Task<int> SynchronizeFavorites(SpotifyClient spotify, SynchronizeFavoritesOptions options)
    {
        CliPrint.PrintInfo("Synchronizing favorites...");
        
        var savedTracks = await new SavedTracksService().GetSavedTracks(spotify);
        
        if (savedTracks == null || savedTracks.Count == 0)
        {
            return 1;
        }
        
        var result = await new FavoritesService().SynchronizeFavorites(
            spotify, 
            savedTracks, 
            options.FavoritesPlaylistId);
        
        return result ? 0 : 1;
    }

    private static async Task<int> SynchronizeArtists(SpotifyClient spotify, SynchronizeArtistsOptions options)
    {
        CliPrint.PrintInfo("Synchronizing artists...");
        
        var savedTracks = await new SavedTracksService().GetSavedTracks(spotify);

        if (savedTracks == null || savedTracks.Count == 0)
        {
            return 1;
        }

        var result = await new ArtistsService().SynchronizeArtists(
            spotify,
            savedTracks);

        return result ? 0 : 1;
    }
}