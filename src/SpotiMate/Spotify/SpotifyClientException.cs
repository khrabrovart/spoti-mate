namespace SpotiMate.Spotify;

public class SpotifyClientException : Exception
{
    public SpotifyClientException(string message) : base(message)
    {
    }
}