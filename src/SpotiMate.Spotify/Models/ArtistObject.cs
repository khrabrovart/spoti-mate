using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models;

public class ArtistObject
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("popularity")]
    public int Popularity { get; set; }

    [JsonPropertyName("followers")]
    public ArtistFollowers Followers { get; set; }
}
