using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models;

public class UserProfile
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; }
}
