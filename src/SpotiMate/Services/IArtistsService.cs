using SpotiMate.Spotify.Models;

namespace SpotiMate.Services;

public interface IArtistsService
{
    Task<bool> FollowArtists(
        IEnumerable<SavedTrackObject> savedTracks,
        TimeSpan recency);
}
