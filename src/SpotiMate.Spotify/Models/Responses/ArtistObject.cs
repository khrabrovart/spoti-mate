using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models.Responses;

public class ArtistObject
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}
