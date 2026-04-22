using SpotiMate.Spotify.Models.Requests;
using SpotiMate.Spotify.Models.Responses;
using SpotiMate.Spotify.Providers;

namespace SpotiMate.Spotify.Apis;

public interface ISpotifyMeApi
{
    Task<ApiResponse<UserProfile>> GetCurrentUserProfile();
    Task<ApiResponse<Page<SavedTrackObject>>> GetSavedTracks(int offset, int limit);
    Task<ApiResponse<FollowedArtistsResponse>> GetFollowedArtists(string after, int limit);
    Task<ApiResponse> SaveLibraryItems(SpotifyLibraryItemType itemType, string[] ids);
    Task<ApiResponse> RemoveLibraryItems(SpotifyLibraryItemType itemType, string[] ids);
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

    public async Task<ApiResponse<FollowedArtistsResponse>> GetFollowedArtists(string after, int limit)
    {
        FieldValidator.Int(nameof(limit), limit, min: 1, max: 50);

        var queryParams = new Dictionary<string, string>
        {
            { "type", "artist" },
            { "limit", limit.ToString() }
        };

        if (!string.IsNullOrEmpty(after))
        {
            queryParams.Add("after", after);
        }

        return await MakeRequest<FollowedArtistsResponse>(
            HttpMethod.Get,
            segment: "following",
            queryParams: queryParams);
    }

    public async Task<ApiResponse> SaveLibraryItems(SpotifyLibraryItemType itemType, string[] ids)
    {
        FieldValidator.Length(nameof(ids), ids, min: 1, max: 40);

        var uris = string.Join(",", ids.Select(id => itemType.ToUri(id)));
        var queryParams = new Dictionary<string, string> { { "uris", uris } };

        return await MakeRequest(
            HttpMethod.Put,
            segment: "library",
            queryParams: queryParams);
    }

    public async Task<ApiResponse> RemoveLibraryItems(SpotifyLibraryItemType itemType, string[] ids)
    {
        FieldValidator.Length(nameof(ids), ids, min: 1, max: 40);

        var uris = string.Join(",", ids.Select(id => itemType.ToUri(id)));
        var queryParams = new Dictionary<string, string> { { "uris", uris } };

        return await MakeRequest(
            HttpMethod.Delete,
            segment: "library",
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
