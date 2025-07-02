using CommandLine;

namespace SpotiMate.Cli;

[Verb("create-blend", HelpText = "Create a new blend playlist with specified options.")]
public class CreateBlendOptions : CliOptions
{
    [Option("blend-refresh-token", Required = true, HelpText = "Spotify refresh token of the blending account.")]
    public string BlendRefreshToken { get; set; }

    [Option("blend-size", Required = true, HelpText = "Number of tracks to include in the blend playlist.")]
    public int BlendSize { get; set; }

    [Option("blend-playlist-id", Required = true, HelpText = "ID of the playlist to add blended tracks to.")]
    public string BlendPlaylistId { get; set; }

    [Option("blend-additional-playlists", Separator = ',', HelpText = "Additional playlists to include in the blend, separated by commas.")]
    public IEnumerable<string> BlendAdditionalPlaylists { get; set; }
}
