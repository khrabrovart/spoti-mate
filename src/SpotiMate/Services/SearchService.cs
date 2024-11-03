using System.Text;
using SpotiMate.Cli;
using SpotiMate.OpenAI.Services;
using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Models;

namespace SpotiMate.Services;

public class SearchService : ISearchService
{
    private readonly ISpotifySearchApi _spotifySearchApi;
    private readonly IOpenAIChatService _openAIChatService;

    private string _prompt;

    public SearchService(ISpotifySearchApi spotifySearchApi, IOpenAIChatService openAIChatService)
    {
        _spotifySearchApi = spotifySearchApi;
        _openAIChatService = openAIChatService;
    }

    public async Task<bool> SearchAndSaveTracks(string[] trackNames, string addToPlaylistId, string openAIApiKey)
    {
        var cache = await ReadCache();

        foreach (var trackName in trackNames)
        {
            if (cache.TryGetValue(trackName, out var cachedTrackId) && !string.IsNullOrEmpty(cachedTrackId))
            {
                continue;
            }

            var searchResponse = await _spotifySearchApi.SearchTracks(trackName, 0, 5);
            var foundTracks = searchResponse.Data.Tracks.Items;

            if (foundTracks.Length == 0)
            {
                CliPrint.Info(trackName);
                await WriteCache(trackName, " ");
                CliPrint.Warning("Track is not found on Spotify, skipping");
                CliPrint.EmptyLine();
                continue;
            }

            var trackSelectionString = ComposeTrackSelectionString(trackName, foundTracks);

            CliPrint.Info(trackSelectionString);

            var (track, isConfident) = await SelectTrackWithOpenAI(trackName, foundTracks, openAIApiKey);

            if (track == null)
            {
                CliPrint.Warning("Track is not selected by OpenAI, please select track manually or skip with Delete key");
            }
            else
            {
                if (!isConfident)
                {
                    CliPrint.Warning("Track is selected by OpenAI with low confidence, please select track manually, confirm OpenAI selection with Enter key or skip with Delete key");
                    CliPrint.Warning(ComposeTrackString(track));
                }
                else
                {
                    CliPrint.Success("Track is selected by OpenAI with high confidence, please confirm OpenAI selection with Enter key, select track manually or skip with Delete key");
                    CliPrint.Success(ComposeTrackString(track));
                }
            }

            var key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    if (track == null)
                    {
                        CliPrint.Warning("Track is not selected by OpenAI and cannot be confirmed, skipping");
                        break;
                    }

                    await WriteCache(trackName, track.Id);
                    CliPrint.Success("Track selection confirmed");
                    break;

                case ConsoleKey.F when key.Modifiers.HasFlag(ConsoleModifiers.Shift):
                    await WriteCache(trackName, " ");
                    CliPrint.Warning("Secret command detected, track selection skipped with empty cache set");
                    break;

                case ConsoleKey.Backspace:
                    CliPrint.Warning("Track selection skipped");
                    break;

                case ConsoleKey.D1:
                case ConsoleKey.D2:
                case ConsoleKey.D3:
                case ConsoleKey.D4:
                case ConsoleKey.D5:
                    var index = int.Parse(key.KeyChar.ToString());

                    if (index < 1 || index > foundTracks.Length)
                    {
                        CliPrint.Warning("Invalid track index, track selection skipped");
                        break;
                    }

                    track = foundTracks[index - 1];
                    await WriteCache(trackName, track.Id);
                    CliPrint.Success(ComposeTrackString(track));
                    CliPrint.Success("Track selection confirmed");
                    break;

                default:
                    CliPrint.Warning("Invalid key pressed, track selection skipped");
                    break;
            }

            CliPrint.EmptyLine();
        }

        return true;
    }

    private static string ComposeTrackSelectionString(string trackName, TrackObject[] foundTracks)
    {
        var trackSelection = new StringBuilder();
        trackSelection.AppendLine(trackName);

        for (var i = 0; i < foundTracks.Length; i++)
        {
            var trackString = ComposeTrackString(foundTracks[i]);

            if (i == foundTracks.Length - 1)
            {
                trackSelection.Append($"{i + 1}) {trackString}");
            }
            else
            {
                trackSelection.AppendLine($"{i + 1}) {trackString}");
            }
        }

        return trackSelection.ToString();
    }

    private static string ComposeTrackString(TrackObject track)
    {
        if (track == null)
        {
            return string.Empty;
        }

        var artists = string.Join(", ", track.Artists.Select(a => a.Name));

        return $"{artists} - {track.Name}";
    }

    private async Task<(TrackObject Track, bool IsConfident)> SelectTrackWithOpenAI(
        string trackName,
        TrackObject[] foundTracks,
        string openAIApiKey)
    {
        var systemMessage = await GetPrompt();
        var userMessage = ComposeTrackSelectionString(trackName, foundTracks);

        var response = await _openAIChatService.Complete(openAIApiKey, systemMessage, userMessage);
        var responseParts = response.Split(" ");
        var responseChoice = responseParts[0];
        var responseConfidence = responseParts[1];

        if (!int.TryParse(responseChoice, out var trackIndex) || trackIndex < 0 || trackIndex > foundTracks.Length)
        {
            trackIndex = 0;
        }

        if (!int.TryParse(responseConfidence, out var confidence) || confidence != 0 && confidence != 1)
        {
            confidence = 0;
        }

        var track = trackIndex == 0 ? null : foundTracks[trackIndex - 1];
        var isConfident = confidence == 1;

        return (track, isConfident);
    }

    private async Task<string> GetPrompt()
    {
        if (_prompt == null)
        {
            var promptFilePath = Path.Combine(Environment.CurrentDirectory, "Resources", "Application", "Search", "prompt.txt");
            _prompt = await File.ReadAllTextAsync(promptFilePath);
        }

        return _prompt;
    }

    private static async Task<Dictionary<string, string>> ReadCache()
    {
        var cacheFilePath = Path.Combine(Environment.CurrentDirectory, "Resources", "Application", "Search", "cache.txt");

        if (!File.Exists(cacheFilePath))
        {
            return new Dictionary<string, string>();
        }

        var cacheData = await File.ReadAllLinesAsync(cacheFilePath);

        return cacheData
            .Select(line => line.Split(" ::= "))
            .ToDictionary(parts => parts[0], parts => parts[1]);
    }

    private static async Task WriteCache(string key, string value)
    {
        var cacheFilePath = Path.Combine(Environment.CurrentDirectory, "Resources", "Application", "Search", "cache.txt");

        var cache = await ReadCache();
        cache[key] = value;

        var cacheData = cache.Select(kvp => $"{kvp.Key} ::= {kvp.Value}");

        await File.WriteAllLinesAsync(cacheFilePath, cacheData);
    }
}
