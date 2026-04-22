using CommandLine;

namespace SpotiMate.Cli;

[Verb("unfollow-artists", HelpText = "Remove all artists you currently follow from your library.")]
public class UnfollowArtistsOptions : CliOptions;
