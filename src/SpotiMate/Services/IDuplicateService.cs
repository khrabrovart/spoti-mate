using SpotiMate.Spotify.Models;

namespace SpotiMate.Services;

public interface IDuplicateService
{
    Task<bool> FindDuplicates(SavedTrackObject[] savedTracks, string duplicatesPlaylistId, TimeSpan recency);
}
