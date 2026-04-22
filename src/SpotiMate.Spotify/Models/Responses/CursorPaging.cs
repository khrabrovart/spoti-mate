using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models.Responses;

public class CursorPaging<TItem>
{
    [JsonPropertyName("href")]
    public string Href { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("next")]
    public string Next { get; set; }

    [JsonPropertyName("cursors")]
    public CursorsObject Cursors { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("items")]
    public TItem[] Items { get; set; }
}
