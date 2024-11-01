using SpotiMate.Spotify.Objects;

namespace SpotiMate.Services;

public interface ISearchService
{
    Task<SpotifySearchTracksObject> SearchTracks();
}
