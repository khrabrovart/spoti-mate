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

    public async Task<bool> FollowArtists(IEnumerable<SavedTrackObject> savedTracks, TimeSpan recency)
    {
        CliPrint.PrintInfo("Following artists");

        var uniqueArtists = savedTracks
            .Where(t => t.AddedAt >= DateTime.UtcNow - recency)
            .SelectMany(t => t.Track.Artists.Select(a => a.Id))
            .Distinct()
            .ToArray();
        
        if (uniqueArtists.Length == 0)
        {
            CliPrint.PrintSuccess("No artists to follow");
            return true;
        }
        
        CliPrint.PrintInfo($"Found {uniqueArtists.Length} unique artists");

        const int chunkSize = 50;
        var chunks = uniqueArtists.Chunk(chunkSize);

        var overallSuccess = true;

        foreach (var chunk in chunks)
        {
            var response = await _spotifyMeApi.FollowArtists(chunk);

            if (!response.IsError)
            {
                continue;
            }

            CliPrint.PrintError($"Failed to follow artists: {response.Error}");
            overallSuccess = false;
        }

        if (overallSuccess)
        {
            CliPrint.PrintSuccess($"Successfully followed {uniqueArtists.Length} artists");
        }

        return overallSuccess;
    }
}
