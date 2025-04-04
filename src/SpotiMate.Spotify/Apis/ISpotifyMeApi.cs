using SpotiMate.Spotify.Models;

namespace SpotiMate.Spotify.Apis;

public interface ISpotifyMeApi
{
    Task<ApiResponse<UserProfile>> GetCurrentUserProfile();

    Task<ApiResponse<Page<SavedTrackObject>>> GetSavedTracks(int offset, int limit);

    Task<ApiResponse<FollowedArtists>> GetFollowedArtists(string after, int limit);

    Task<ApiResponse> FollowArtists(string[] artistIds);

    Task<ApiResponse> UnfollowArtists(string[] artistIds);
}
