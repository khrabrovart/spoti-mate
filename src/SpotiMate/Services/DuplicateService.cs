using SpotiMate.Cli;
using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Models;

namespace SpotiMate.Services;

public class DuplicateService : IDuplicateService
{
    private class PreparedTrack
    {
        public string Id { get; set; }

        public string OriginalName { get; set; }

        public string SanitizedName { get; set; }

        public string[] ArtistNames { get; set; }

        public string[] ArtistIds { get; set; }

        public int Popularity { get; set; }

        public DateTime AddedAt { get; set; }
    }

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
            
            var localDuplicates = new List<PreparedTrack>();
            
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
                CliPrint.Info($"- {string.Join(", ", duplicate.ArtistNames)} - {duplicate.OriginalName}; Id {duplicate.Id}; Added on {duplicate.AddedAt}");
            }
                
            var orderedDuplicates = localDuplicates
                .OrderByDescending(d => d.Popularity)
                .Select(d => d.Id);
                
            duplicates.AddRange(orderedDuplicates);
        }
        
        return duplicates.Distinct().ToArray();
    }
    
    private static PreparedTrack PrepareTrack(SavedTrackObject track)
    {
        return new PreparedTrack
        {
            Id = track.Track.Id,
            OriginalName = track.Track.Name,
            SanitizedName = PrepareTrackName(track.Track.Name),
            ArtistNames = track.Track.Artists.Select(a => a.Name).ToArray(),
            ArtistIds = track.Track.Artists.Select(a => a.Id).ToArray(),
            Popularity = track.Track.Popularity,
            AddedAt = track.AddedAt
        };
    }
    
    private static string PrepareTrackName(string trackName)
    {
        var lower = trackName.ToLowerInvariant();
        var words = lower
            .Split(' ')
            .TakeWhile(w => char.IsLetterOrDigit(w[0]));

        return string.Join(" ", words);
    }

    private static bool TracksAreSimilar(PreparedTrack a, PreparedTrack b)
    {
        return a.SanitizedName == b.SanitizedName && a.ArtistIds.Intersect(b.ArtistIds).Any();
    }
}
