using SpotiMate.Cli;
using SpotiMate.Services;

namespace SpotiMate.Handlers;

public class CommandHandler : ICommandHandler
{
    private readonly ISavedTracksService _savedTracksService;
    private readonly IDuplicatesService _duplicatesService;
    private readonly IArtistsService _artistsService;
    private readonly ISearchService _searchService;

    public CommandHandler(
        ISavedTracksService savedTracksService,
        IDuplicatesService duplicatesService,
        IArtistsService artistsService,
        ISearchService searchService)
    {
        _savedTracksService = savedTracksService;
        _duplicatesService = duplicatesService;
        _artistsService = artistsService;
        _searchService = searchService;
    }

    public async Task<int> Handle(CliOptions options)
    {
        return options switch
        {
            RunAllOptions runAllOptions => await RunAll(runAllOptions),
            SearchTracksOptions searchTracksOptions => await SearchTracks(searchTracksOptions),
            _ => throw new InvalidOperationException()
        };
    }

    private async Task<int> RunAll(RunAllOptions options)
    {
        var savedTracks = await _savedTracksService.GetSavedTracks();

        if (savedTracks == null || savedTracks.Length == 0)
        {
            return 1;
        }

        var duplicatesProcessed = await _duplicatesService.FindDuplicates(
            savedTracks,
            options.DuplicatesPlaylistId,
            TimeSpan.FromDays(options.Days));

        var artistsSynchronized = await _artistsService.FollowArtists(
            savedTracks,
            TimeSpan.FromDays(options.Days));

        return duplicatesProcessed && artistsSynchronized ? 0 : 1;
    }

    private async Task<int> SearchTracks(SearchTracksOptions options)
    {
        var filePath = Path.Combine(Environment.CurrentDirectory, "Resources", "User", "playlist.txt");
        var trackNames = await File.ReadAllLinesAsync(filePath);

        var success = await _searchService.SearchAndSaveTracks(trackNames, options.AddToPlaylistId, options.OpenAIApiKey);

        return success ? 0 : 1;
    }
}
