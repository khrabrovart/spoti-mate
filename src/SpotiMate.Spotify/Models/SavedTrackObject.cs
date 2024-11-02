using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models;

public class SavedTrackObject
{
    [JsonPropertyName("added_at")]
    public DateTime AddedAt { get; set; }
    
    [JsonPropertyName("track")]
    public TrackObject Track { get; set; }
}
