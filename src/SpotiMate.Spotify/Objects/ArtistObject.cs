using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Objects;

public class ArtistObject
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
}