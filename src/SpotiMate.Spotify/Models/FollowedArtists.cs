using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models;

public class FollowedArtists
{
    [JsonPropertyName("artists")]
    public Page<ArtistObject> Artists { get; set; }
}
