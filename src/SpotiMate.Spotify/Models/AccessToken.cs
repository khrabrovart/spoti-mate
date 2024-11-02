namespace SpotiMate.Spotify.Models;

public class AccessToken
{
    private readonly string _value;

    private AccessToken(string value)
    {
        _value = value;
    }

    public static implicit operator string(AccessToken accessToken)
    {
        return accessToken._value;
    }

    public static implicit operator AccessToken(string accessToken)
    {
        return new AccessToken(accessToken);
    }
}
