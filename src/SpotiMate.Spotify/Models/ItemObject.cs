using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models;

public class ItemObject
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("artists")]
    public ArtistObject[] Artists { get; set; }
    
    [JsonPropertyName("duration_ms")]
    public int DurationMs { get; set; }
}
