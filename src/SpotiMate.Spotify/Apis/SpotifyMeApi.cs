using SpotiMate.Spotify.Models;
using SpotiMate.Spotify.Providers;

namespace SpotiMate.Spotify.Apis;

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

    public async Task<ApiResponse> FollowArtists(string[] artistIds)
    {
        FieldValidator.Length(nameof(artistIds), artistIds, min: 0, max: 50);

        var queryParams = new Dictionary<string, string>
        {
            { "type", "artist" }
        };

        var body = new
        {
            ids = artistIds
        };

        return await MakeRequest(
            HttpMethod.Put,
            segment: "following",
            queryParams: queryParams,
            body: body);
    }
}
