using SpotiMate.Cli;
using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Models;

namespace SpotiMate.Services;

public class DuplicateService : IDuplicateService
{
    private readonly ISpotifyPlaylistsApi _spotifyPlaylistsApi;

    public DuplicateService(ISpotifyPlaylistsApi spotifyPlaylistsApi)
    {
        _spotifyPlaylistsApi = spotifyPlaylistsApi;
    }

    public async Task<bool> FindDuplicates(SavedTrackObject[] savedTracks, string duplicatesPlaylistId, TimeSpan recency)
    {
        CliPrint.Info("Checking duplicates");
        
        var duplicates = GetDuplicates(savedTracks, recency);
        
        if (duplicates.Length == 0)
        {
            return true;
        }

        const int chunkSize = 100;
        var chunks = duplicates.Chunk(chunkSize);

        var overallSuccess = true;

        foreach (var chunk in chunks)
        {
            var result = await _spotifyPlaylistsApi.AddTracksToPlaylist(duplicatesPlaylistId, chunk);

            if (!result.IsError)
            {
                continue;
            }

            CliPrint.Error($"Failed to add duplicates to playlist: {result.Error}");
            overallSuccess = false;
        }

        if (overallSuccess)
        {
            CliPrint.Success($"Successfully saved {duplicates.Length} duplicates");
        }

        return overallSuccess;
    }
    
    private static string[] GetDuplicates(IEnumerable<SavedTrackObject> savedTracks, TimeSpan recency)
    {
        var preparedTracks = savedTracks
            .Select(PrepareTrack)
            .ToArray();

        var duplicates = new List<string>();
        
        for (var i = 0; i < preparedTracks.Length; i++)
        {
            var a = preparedTracks[i];
            
            if (a.AddedAt < DateTime.UtcNow - recency)
            {
                continue;
            }
            
            var localDuplicates = new List<(string Id, string Name, string[] Artists, int Popularity, DateTime AddedAt)>();
            
            for (var j = i + 1; j < preparedTracks.Length; j++)
            {
                var b = preparedTracks[j];

                if (TracksAreSimilar(a, b))
                {
                    localDuplicates.Add(b);
                }
            }

            if (localDuplicates.Count == 0)
            {
                continue;
            }
            
            localDuplicates.Add(a);

            foreach (var duplicate in localDuplicates)
            {
                CliPrint.Info($"- {string.Join(", ", duplicate.Artists)} - {duplicate.Name}; Id {duplicate.Id}; Added on {duplicate.AddedAt}");
            }
                
            var orderedDuplicates = localDuplicates
                .OrderByDescending(d => d.Popularity)
                .Select(d => d.Id);
                
            duplicates.AddRange(orderedDuplicates);
        }
        
        return duplicates.Distinct().ToArray();
    }
    
    private static (string Id, string Name, string[] Artists, int Popularity, DateTime AddedAt) PrepareTrack(SavedTrackObject track)
    {
        return (
            track.Track.Id,
            PrepareTrackName(track.Track.Name),
            track.Track.Artists.Select(a => a.Id).ToArray(),
            track.Track.Popularity,
            track.AddedAt);
    }
    
    private static string PrepareTrackName(string trackName)
    {
        var lower = trackName.ToLowerInvariant();
        var words = lower
            .Split(' ')
            .TakeWhile(w => char.IsLetterOrDigit(w[0]));

        return string.Join(" ", words);
    }

    private static bool TracksAreSimilar(
        (string Id, string Name, string[] Artists, int Popularity, DateTime AddedAt) a,
        (string Id, string Name, string[] Artists, int Popularity, DateTime AddedAt) b)
    {
        return a.Name == b.Name && a.Artists.Intersect(b.Artists).Any();
    }
}
