namespace SpotiMate.Spotify.Providers;

public interface ISpotifyAuthProvider
{
    Task<string> GetAccessToken();
}
