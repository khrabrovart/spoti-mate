using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models.Responses;

public class FollowedArtistsResponse
{
    [JsonPropertyName("artists")]
    public CursorPaging<ArtistObject> Artists { get; set; }
}
