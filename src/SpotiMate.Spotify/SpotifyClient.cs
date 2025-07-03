using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Providers;
using SpotiMate.Spotify.Services;

namespace SpotiMate.Spotify;

public interface ISpotifyClient
{
    ISpotifyArtistsService Artists { get; }
    ISpotifyMeService Me { get; }
    ISpotifyPlaylistsService Playlists { get; }
    ISpotifyTracksService Tracks { get; }
}

public class SpotifyClient : ISpotifyClient
{
    public SpotifyClient(string clientId, string clientSecret, string refreshToken)
    {
        var authProvider = new SpotifyAuthProvider(clientId, clientSecret, refreshToken);

        Artists = new SpotifyArtistsService(new SpotifyArtistsApi(authProvider));
        Me = new SpotifyMeService(new SpotifyMeApi(authProvider));
        Playlists = new SpotifyPlaylistsService(new SpotifyPlaylistsApi(authProvider));
        Tracks = new SpotifyTracksService(new SpotifyTracksApi(authProvider));
    }

    public ISpotifyArtistsService Artists { get; }
    public ISpotifyMeService Me { get; }
    public ISpotifyPlaylistsService Playlists { get; }
    public ISpotifyTracksService Tracks { get; }
}
