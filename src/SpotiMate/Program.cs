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
            return await Parser.Default
                .ParseArguments<FindDuplicatesOptions, SynchronizeArtistsOptions>(args)
                .MapResult(
                    (FindDuplicatesOptions options) => FindDuplicates(options),
                    (SynchronizeArtistsOptions options) => SynchronizeArtists(options),
                    _ => Task.FromResult(1));
        }
        catch (Exception ex)
        {
            CliPrint.PrintError(ex.Message);
            return 1;
        }
    }
    
    private static async Task<int> FindDuplicates(FindDuplicatesOptions options)
    {
        var spotify = await GetSpotifyClient(options);
        
        CliPrint.PrintInfo($"Finding duplicates for the last {options.Days} days...");
        
        var savedTracks = await new SavedTracksService().GetSavedTracks(spotify);
        
        if (savedTracks == null || savedTracks.Length == 0)
        {
            return 1;
        }
        
        var result = await new DuplicatesService().FindDuplicates(
            spotify, 
            savedTracks, 
            options.DuplicatesPlaylistId,
            TimeSpan.FromDays(options.Days));
        
        return result ? 0 : 1;
    }

    private static async Task<int> SynchronizeArtists(SynchronizeArtistsOptions options)
    {
        var spotify = await GetSpotifyClient(options);
        
        CliPrint.PrintInfo($"Synchronizing artists for the last {options.Days} days...");
        
        var savedTracks = await new SavedTracksService().GetSavedTracks(spotify);

        if (savedTracks == null || savedTracks.Length == 0)
        {
            return 1;
        }

        var result = await new ArtistsService().SynchronizeArtists(
            spotify,
            savedTracks,
            TimeSpan.FromDays(options.Days));

        return result ? 0 : 1;
    }
    
    private static async Task<SpotifyClient> GetSpotifyClient(CliOptions options)
    {
        var spotify = new SpotifyClient();
        await spotify.Authorize(options.ClientId, options.ClientSecret, options.RefreshToken);
        
        return spotify;
    }
}