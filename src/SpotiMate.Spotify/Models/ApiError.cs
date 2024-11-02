using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models;

public class ApiError
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}
