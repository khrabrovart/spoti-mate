using SpotiMate.Cli;
using SpotiMate.Spotify;
using SpotiMate.Spotify.Objects;

namespace SpotiMate.Services;

public class SavedTracksService
{
    public async Task<SavedTrackObject[]> GetSavedTracks(SpotifyClient spotify)
    {
        CliPrint.PrintInfo("Loading saved tracks...");
        var savedTracks = await spotify.GetSavedTracks();
        
        if (savedTracks == null)
        {
            CliPrint.PrintError("Failed to load saved tracks.");
            return null;
        }
        
        CliPrint.PrintInfo($"Loaded {savedTracks.Length} saved tracks.");
        return savedTracks;
    }
}