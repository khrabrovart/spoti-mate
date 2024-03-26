using CommandLine;

namespace SpotiMate.Cli;

[Verb("synchronize-favorites", HelpText = "Synchronize favorites.")]
public class SynchronizeFavoritesOptions : CliOptions
{
    [Option("favorites-playlist-id", Required = true, HelpText = "Spotify favorites playlist id.")]
    public string FavoritesPlaylistId { get; set; }
}