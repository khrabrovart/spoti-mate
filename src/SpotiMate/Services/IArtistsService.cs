using SpotiMate.Spotify.Models;

namespace SpotiMate.Services;

public interface IArtistsService
{
    Task<bool> SynchronizeArtists(
        IEnumerable<SavedTrackObject> savedTracks,
        TimeSpan recency);
}
