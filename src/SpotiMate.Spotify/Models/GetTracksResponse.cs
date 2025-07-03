using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models;

public class GetTracksResponse
{
    [JsonPropertyName("tracks")]
    public TrackObject[] Tracks { get; set; }
}
