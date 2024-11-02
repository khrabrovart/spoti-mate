using CommandLine;

namespace SpotiMate.Cli;

[Verb("run-all", HelpText = "Run all commands one by one.")]
public class RunAllOptions : CliOptions
{
    [Option("days", Required = true, HelpText = "Number of days to run commands for.")]
    public int Days { get; set; }
    
    [Option("duplicates-playlist-id", Required = true, HelpText = "ID of the playlist to add duplicates to.")]
    public string DuplicatesPlaylistId { get; set; }
}
