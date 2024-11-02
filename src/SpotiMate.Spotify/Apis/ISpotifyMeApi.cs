using SpotiMate.Spotify.Models;

namespace SpotiMate.Spotify.Apis;

public interface ISpotifyMeApi
{
    Task<ApiResponse<Page<SavedTrackObject>>> GetSavedTracks(int offset, int limit);

    Task<ApiResponse> FollowArtists(string[] artistIds);
}
