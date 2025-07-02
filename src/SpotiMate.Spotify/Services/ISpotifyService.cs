namespace SpotiMate.Spotify.Services;

public interface ISpotifyService<out TApi>
{
    TApi Api { get; }
}
