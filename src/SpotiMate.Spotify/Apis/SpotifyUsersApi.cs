using SpotiMate.Spotify.Models;
using SpotiMate.Spotify.Providers;

namespace SpotiMate.Spotify.Apis;

public class SpotifyUsersApi : SpotifyApiBase, ISpotifyUsersApi
{
    public SpotifyUsersApi(ISpotifyAuthProvider authProvider) : base(authProvider, "users")
    {
    }

    public async Task<ApiResponse<Playlist>> CreatePlaylist(string userId, string playlistName)
    {
        var body = new
        {
            name = playlistName
        };

        return await MakeRequest<Playlist>(
            HttpMethod.Post,
            segment: $"{userId}/playlists",
            body: body);
    }
}
