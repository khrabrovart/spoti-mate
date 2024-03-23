namespace SpotiMate.Spotify.Extensions;

public static class StringExtensions
{
    public static string ToSpotifyTrackUri(this string trackId) => $"spotify:track:{trackId}";
}