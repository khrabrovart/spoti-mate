using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models;

public class Playlist
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("public")]
    public bool Public { get; set; }
}
