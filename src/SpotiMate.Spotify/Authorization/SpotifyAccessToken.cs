namespace SpotiMate.Spotify.Authorization;

public class SpotifyAccessToken
{
    private readonly string _value;

    private SpotifyAccessToken(string value)
    {
        _value = value;
    }

    public static implicit operator string(SpotifyAccessToken accessToken)
    {
        return accessToken._value;
    }

    public static implicit operator SpotifyAccessToken(string accessToken)
    {
        return new SpotifyAccessToken(accessToken);
    }
}
