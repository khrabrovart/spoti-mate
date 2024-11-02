using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models;

public class SearchResults
{
    [JsonPropertyName("tracks")]
    public Page<TrackObject> Tracks { get; set; }
}
