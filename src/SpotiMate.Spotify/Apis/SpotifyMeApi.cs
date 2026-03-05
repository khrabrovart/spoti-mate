using SpotiMate.Spotify.Models;
using SpotiMate.Spotify.Providers;

namespace SpotiMate.Spotify.Apis;

public interface ISpotifyMeApi
{
    Task<ApiResponse<UserProfile>> GetCurrentUserProfile();
    Task<ApiResponse<Page<SavedTrackObject>>> GetSavedTracks(int offset, int limit);
    Task<ApiResponse<Playlist>> CreatePlaylist(string playlistName);
}

public class SpotifyMeApi : SpotifyApiBase, ISpotifyMeApi
{
    public SpotifyMeApi(ISpotifyAuthProvider authProvider) : base(authProvider, "me")
    {
    }

    public async Task<ApiResponse<UserProfile>> GetCurrentUserProfile()
    {
        return await MakeRequest<UserProfile>(HttpMethod.Get);
    }

    public async Task<ApiResponse<Page<SavedTrackObject>>> GetSavedTracks(int offset, int limit)
    {
        FieldValidator.Int(nameof(offset), offset, min: 0);
        FieldValidator.Int(nameof(limit), limit, min: 0, max: 50);

        var queryParams = new Dictionary<string, string>
        {
            { "offset", offset.ToString() },
            { "limit", limit.ToString() }
        };

        return await MakeRequest<Page<SavedTrackObject>>(
            HttpMethod.Get,
            segment: "tracks",
            queryParams: queryParams);
    }

    public async Task<ApiResponse<Playlist>> CreatePlaylist(string playlistName)
    {
        var body = new
        {
            name = playlistName
        };

        return await MakeRequest<Playlist>(
            HttpMethod.Post,
            segment: "playlists",
            body: body);
    }
}
