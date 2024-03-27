using CommandLine;

namespace SpotiMate.Cli;

[Verb("find-duplicates", HelpText = "Find duplicates in saved tracks.")]
public class FindDuplicatesOptions : CliOptions
{
    [Option("days", Required = true, HelpText = "Number of days to search for duplicates.")]
    public int Days { get; set; }
    
    [Option("duplicates-playlist-id", Required = true, HelpText = "ID of the playlist to add duplicates to.")]
    public string DuplicatesPlaylistId { get; set; }
}