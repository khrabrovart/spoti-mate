using SpotiMate.Spotify.Models;

namespace SpotiMate.Services;

public interface IArtistService
{
    Task<bool> SyncArtists(IEnumerable<SavedTrackObject> savedTracks);
}
