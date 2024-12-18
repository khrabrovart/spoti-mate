using System.Text.Json.Serialization;

namespace SpotiMate.Spotify.Models;

public class AuthorizationData
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
    
    [JsonPropertyName("scope")]
    public string Scope { get; set; }
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
