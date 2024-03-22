using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Objects;

public class TrackObject
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("artists")]
    public ArtistObject[] Artists { get; set; }
    
    [JsonPropertyName("popularity")]
    public int Popularity { get; set; }
}