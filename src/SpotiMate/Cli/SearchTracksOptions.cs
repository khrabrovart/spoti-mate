using CommandLine;

namespace SpotiMate.Cli;

[Verb("search-tracks", HelpText = "Search for provided track names.")]
public class SearchTracksOptions : CliOptions
{
}
