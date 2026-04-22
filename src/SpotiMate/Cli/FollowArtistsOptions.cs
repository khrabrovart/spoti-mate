using CommandLine;

namespace SpotiMate.Cli;

[Verb("follow-artists", HelpText = "Add all unique artists from saved tracks to your library.")]
public class FollowArtistsOptions : CliOptions;
