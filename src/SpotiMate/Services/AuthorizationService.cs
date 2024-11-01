using SpotiMate.Cli;
using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Authorization;
using SpotiMate.Spotify.Responses;

namespace SpotiMate.Services;

public class AuthorizationService
{
    private readonly SpotifyAccessToken _accessToken;

    public AuthorizationService(SpotifyAccessToken accessToken)
    {
        _accessToken = accessToken;
    }
}
