using SpotiMate.Cli;
using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Models;

namespace SpotiMate.Services;

public class ArtistService : IArtistService
{
    private readonly ISpotifyMeApi _spotifyMeApi;

    public ArtistService(ISpotifyMeApi spotifyMeApi)
    {
        _spotifyMeApi = spotifyMeApi;
    }

    public async Task<bool> FollowArtists(IEnumerable<SavedTrackObject> savedTracks, TimeSpan recency)
    {
        CliPrint.Info("Following artists");

        var uniqueArtists = savedTracks
            .Where(t => t.AddedAt >= DateTime.UtcNow - recency)
            .SelectMany(t => t.Track.Artists.Select(a => a.Id))
            .Distinct()
            .ToArray();
        
        if (uniqueArtists.Length == 0)
        {
            CliPrint.Success("No artists to follow");
            return true;
        }
        
        CliPrint.Info($"Found {uniqueArtists.Length} unique artists");

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

            CliPrint.Error($"Failed to follow artists: {response.Error}");
            overallSuccess = false;
        }

        if (overallSuccess)
        {
            CliPrint.Success($"Successfully followed {uniqueArtists.Length} artists");
        }

        return overallSuccess;
    }
}
