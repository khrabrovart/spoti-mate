using SpotiMate.Cli;
using SpotiMate.Spotify;
using SpotiMate.Spotify.Objects;

namespace SpotiMate.Services;

public class ArtistsService
{
    public async Task<bool> SynchronizeArtists(
        SpotifyClient spotify, 
        IEnumerable<SavedTrackObject> savedTracks)
    {
        var uniqueArtists = savedTracks
            .SelectMany(t => t.Track.Artists.Select(a => a.Id))
            .Distinct()
            .ToArray();
        
        if (uniqueArtists.Length == 0)
        {
            CliPrint.PrintSuccess("No artists to synchronize.");
            return true;
        }
        
        CliPrint.PrintInfo($"Following {uniqueArtists.Length} artists...");
        
        var followed = await spotify.FollowArtists(uniqueArtists);
        
        if (followed)
        {
            CliPrint.PrintSuccess("Artists synchronized.");
        }
        else
        {
            CliPrint.PrintError("Failed to synchronize artists.");
        }
        
        return followed;
    }
}