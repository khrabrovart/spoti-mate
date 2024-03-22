using CommandLine;

namespace SpotiMate.Cli;

public class CliOptions
{
    [Option("client-id", Required = true, HelpText = "Spotify client id.")]
    public string ClientId { get; set; }
    
    [Option("client-secret", Required = true, HelpText = "Spotify client secret.")]
    public string ClientSecret { get; set; }
    
    [Option("refresh-token", Required = true, HelpText = "Spotify refresh token.")]
    public string RefreshToken { get; set; }
    
    [Option("favorites-playlist-id", Required = true, HelpText = "Spotify favorites playlist id.")]
    public string FavoritesPlaylistId { get; set; }
}