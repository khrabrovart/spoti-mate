using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models.Responses;

public class PlaylistItemObject
{
    [JsonPropertyName("added_at")]
    public DateTime AddedAt { get; set; }

    [JsonPropertyName("item")]
    public ItemObject Item { get; set; }
}
