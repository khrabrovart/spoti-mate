using System.Text.Json.Serialization;
using SpotiMate.Spotify.Objects;

namespace SpotiMate.Spotify.Responses;

public class SpotifySearchResponse
{
    [JsonPropertyName("tracks")]
    public SpotifySearchTracksObject Tracks { get; set; }
}
