using SpotiMate.Spotify.Models;

namespace SpotiMate.Services;

public interface IArtistService
{
    Task<bool> FollowArtists(
        IEnumerable<SavedTrackObject> savedTracks,
        TimeSpan recency);
}
