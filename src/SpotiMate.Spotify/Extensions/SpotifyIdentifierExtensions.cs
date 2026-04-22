using SpotiMate.Spotify.Models.Requests;

namespace SpotiMate.Spotify.Extensions;

static internal class SpotifyIdentifierExtensions
{
    public static string ToSpotifyTrackUri(this string trackId) => SpotifyLibraryItemType.Track.ToUri(trackId);
}
