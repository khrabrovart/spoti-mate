using System.Text.Json.Serialization;
using SpotiMate.Spotify.Objects;

namespace SpotiMate.Spotify.Responses;

public class GetPlaylistItemsResponse
{
    [JsonPropertyName("limit")]
    public int Limit { get; set; }
    
    [JsonPropertyName("offset")]
    public int Offset { get; set; }
    
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("items")]
    public PlaylistTrackObject[] Items { get; set; }
}