using SpotiMate.Cli;
using SpotiMate.Spotify;
using SpotiMate.Spotify.Objects;

namespace SpotiMate.Services;

public class DuplicatesService
{
    public async Task<bool> FindDuplicates(
        SpotifyClient spotify, 
        SavedTrackObject[] savedTracks, 
        string duplicatesPlaylistId,
        TimeSpan recency)
    {
        CliPrint.PrintInfo("Checking for duplicates...");
        
        var duplicates = GetDuplicates(savedTracks, recency);
        
        if (duplicates.Count == 0)
        {
            CliPrint.PrintInfo("No duplicates found.");
            return true;
        }
        
        CliPrint.PrintInfo($"Found {duplicates.Count} duplicates.");
        CliPrint.PrintInfo("Adding duplicates to playlist...");
        var result = await spotify.AddTracksToPlaylist(duplicatesPlaylistId, duplicates);
        
        if (result)
        {
            CliPrint.PrintSuccess("Duplicates added to playlist.");
        }
        else
        {
            CliPrint.PrintError("Failed to add duplicates to playlist.");
        }
        
        return result;
    }
    
    private static List<string> GetDuplicates(IEnumerable<SavedTrackObject> savedTracks, TimeSpan recency)
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
                
                if (a.Name != b.Name || !a.Artists.Intersect(b.Artists).Any())
                {
                    continue;
                }
                
                localDuplicates.Add(b);
            }

            if (localDuplicates.Count <= 0)
            {
                continue;
            }
            
            localDuplicates.Add(a);
                
            var orderedDuplicates = localDuplicates
                .OrderByDescending(d => d.Popularity)
                .Select(d => d.Id);
                
            duplicates.AddRange(orderedDuplicates);
        }
        
        return duplicates;
    }
    
    private static (string Id, string Name, string[] Artists, int Popularity, DateTime AddedAt) PrepareTrack(SavedTrackObject track)
    {
        var trackName = PrepareTrackName(track.Track.Name);
        
        return (
            track.Track.Id,
            trackName,
            track.Track.Artists.Select(a => a.Id).ToArray(),
            track.Track.Popularity,
            track.AddedAt);
    }
    
    private static string PrepareTrackName(string trackName)
    {
        var lower = trackName.ToLowerInvariant();
        var nameWords = lower
            .Split(' ')
            .TakeWhile(w => char.IsLetterOrDigit(w[0]));

        return string.Join(" ", nameWords);
    }
}