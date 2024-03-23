using SpotiMate.Spotify.Objects;

namespace SpotiMate.Spotify.Responses;

public interface ISpotifyPageResponse<T> where T : ISpotifyObject
{
    int Limit { get; }
    
    int Offset { get; }
    
    int Total { get; }
    
    T[] Items { get; }
}