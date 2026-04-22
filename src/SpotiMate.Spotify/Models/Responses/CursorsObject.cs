using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models.Responses;

public class CursorsObject
{
    [JsonPropertyName("after")]
    public string After { get; set; }

    [JsonPropertyName("before")]
    public string Before { get; set; }
}
