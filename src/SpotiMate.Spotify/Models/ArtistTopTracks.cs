using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models;

public class ArtistTopTracks
{
    [JsonPropertyName("tracks")]
    public TrackObject[] Tracks { get; set; }
}
