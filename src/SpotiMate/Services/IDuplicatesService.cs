using SpotiMate.Spotify.Models;

namespace SpotiMate.Services;

public interface IDuplicatesService
{
    Task<bool> FindDuplicates(SavedTrackObject[] savedTracks, string duplicatesPlaylistId, TimeSpan recency);
}
