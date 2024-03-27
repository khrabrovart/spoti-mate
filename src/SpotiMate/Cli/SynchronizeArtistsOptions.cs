using CommandLine;

namespace SpotiMate.Cli;

[Verb("synchronize-artists", HelpText = "Synchronize artists.")]
public class SynchronizeArtistsOptions : CliOptions
{
    [Option("days", Required = true, HelpText = "Number of days to synchronize.")]
    public int Days { get; set; }
}