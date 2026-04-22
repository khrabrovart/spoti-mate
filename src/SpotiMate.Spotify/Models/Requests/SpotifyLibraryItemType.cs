namespace SpotiMate.Spotify.Models.Requests;

public sealed class SpotifyLibraryItemType
{
    public static SpotifyLibraryItemType Track { get; } = new SpotifyLibraryItemType("track");

    public static SpotifyLibraryItemType Album { get; } = new SpotifyLibraryItemType("album");

    public static SpotifyLibraryItemType Artist { get; } = new SpotifyLibraryItemType("artist");

    public static SpotifyLibraryItemType Episode { get; } = new SpotifyLibraryItemType("episode");

    public static SpotifyLibraryItemType Show { get; } = new SpotifyLibraryItemType("show");

    public static SpotifyLibraryItemType Audiobook { get; } = new SpotifyLibraryItemType("audiobook");

    public static SpotifyLibraryItemType User { get; } = new SpotifyLibraryItemType("user");

    public static SpotifyLibraryItemType Playlist { get; } = new SpotifyLibraryItemType("playlist");

    private readonly string _uriSegment;

    private SpotifyLibraryItemType(string uriSegment)
    {
        _uriSegment = uriSegment;
    }

    public string ToUri(string spotifyId) => $"spotify:{_uriSegment}:{spotifyId}";
}
