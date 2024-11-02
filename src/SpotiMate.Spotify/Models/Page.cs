using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models;

public class Page<TItem>
{
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("items")]
    public TItem[] Items { get; set; }
}
