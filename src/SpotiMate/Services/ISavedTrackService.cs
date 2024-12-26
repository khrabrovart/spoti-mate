using SpotiMate.Spotify.Models;

namespace SpotiMate.Services;

public interface ISavedTrackService
{
    Task<SavedTrackObject[]> GetSavedTracks();
}
