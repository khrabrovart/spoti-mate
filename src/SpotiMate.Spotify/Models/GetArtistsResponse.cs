using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models;

public class GetArtistsResponse
{
    [JsonPropertyName("artists")]
    public ArtistObject[] Artists { get; set; }
}
