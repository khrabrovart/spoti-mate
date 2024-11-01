namespace SpotiMate.Spotify.Extensions;

static internal class StringExtensions
{
    public static string ToSpotifyTrackUri(this string trackId) => $"spotify:track:{trackId}";
}
