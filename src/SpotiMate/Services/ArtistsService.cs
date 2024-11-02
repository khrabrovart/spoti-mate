using SpotiMate.Cli;
using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Models;

namespace SpotiMate.Services;

public class ArtistsService : IArtistsService
{
    private readonly ISpotifyMeApi _spotifyMeApi;

    public ArtistsService(ISpotifyMeApi spotifyMeApi)
    {
        _spotifyMeApi = spotifyMeApi;
    }

    public async Task<bool> SynchronizeArtists(
        IEnumerable<SavedTrackObject> savedTracks,
        TimeSpan recency)
    {
        var uniqueArtists = savedTracks
            .Where(t => t.AddedAt >= DateTime.UtcNow - recency)
            .SelectMany(t => t.Track.Artists.Select(a => a.Id))
            .Distinct()
            .ToArray();
        
        if (uniqueArtists.Length == 0)
        {
            CliPrint.PrintSuccess("No artists to synchronize");
            return true;
        }
        
        CliPrint.PrintInfo($"Following {uniqueArtists.Length} artists");
        
        var followed = await _spotifyMeApi.FollowArtists(uniqueArtists);
        
        if (!followed.IsError)
        {
            CliPrint.PrintSuccess("Artists synchronized");
        }
        else
        {
            CliPrint.PrintError("Failed to synchronize artists");
        }
        
        return !followed.IsError;
    }
}
