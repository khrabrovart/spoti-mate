using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models;

public class ArtistFollowers
{
    [JsonPropertyName("total")]
    public int Total { get; set; }
}
