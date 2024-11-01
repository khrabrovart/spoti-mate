using System.Text.Json.Serialization;
using SpotiMate.Spotify.Responses;

namespace SpotiMate.Spotify.Objects;

public class SearchTracksObject : ISpotifyPageResponse<TrackObject>
{
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("items")]
    public TrackObject[] Items { get; set; }
}
