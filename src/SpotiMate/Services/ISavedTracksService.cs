using SpotiMate.Spotify.Models;

namespace SpotiMate.Services;

public interface ISavedTracksService
{
    Task<SavedTrackObject[]> GetSavedTracks();
}
