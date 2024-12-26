using CommandLine;

namespace SpotiMate.Cli;

[Verb("search-tracks", HelpText = "Search for provided track names and add them to a playlist.")]
public class SearchTracksOptions : CliOptions
{
    [Option("openai-api-key", Required = true, HelpText = "OpenAI API key.")]
    public string OpenAIApiKey { get; set; }
}
