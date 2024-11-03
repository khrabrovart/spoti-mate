using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models;

public class ApiError
{
    [JsonPropertyName("message")]
    public string Message { get; set; }
}
