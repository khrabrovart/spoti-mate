using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models;

public class PlaylistTrackObject
{
    [JsonPropertyName("added_at")]
    public DateTime AddedAt { get; set; }
    
    [JsonPropertyName("track")]
    public TrackObject Track { get; set; }
}
