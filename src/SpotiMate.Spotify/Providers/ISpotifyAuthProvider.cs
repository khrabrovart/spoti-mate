using SpotiMate.Spotify.Models;

namespace SpotiMate.Spotify.Providers;

public interface ISpotifyAuthProvider
{
    Task<AccessToken> GetAccessToken();
}