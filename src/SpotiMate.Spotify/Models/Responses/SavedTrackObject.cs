using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models.Responses;

public class SavedTrackObject
{
    [JsonPropertyName("added_at")]
    public DateTime AddedAt { get; set; }

    [JsonPropertyName("track")]
    public ItemObject Item { get; set; }
}
