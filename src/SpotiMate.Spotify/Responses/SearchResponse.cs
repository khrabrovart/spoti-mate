using System.Text.Json.Serialization;
using SpotiMate.Spotify.Objects;

namespace SpotiMate.Spotify.Responses;

public class SearchResponse
{
    [JsonPropertyName("tracks")]
    public SearchTracksObject Tracks { get; set; }
}
