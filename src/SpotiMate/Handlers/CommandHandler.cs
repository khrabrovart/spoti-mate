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
            SearchTracksOptions => await SearchTracks(),
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

        var duplicatesFound = await _duplicatesService.FindDuplicates(
            savedTracks,
            options.DuplicatesPlaylistId,
            TimeSpan.FromDays(options.Days));


        if (!duplicatesFound)
        {
            return 1;
        }

        CliPrint.PrintInfo($"Synchronizing artists for the last {options.Days} days");

        var artistsSynchronized = await _artistsService.SynchronizeArtists(
            savedTracks,
            TimeSpan.FromDays(options.Days));

        return artistsSynchronized ? 0 : 1;
    }

    private async Task<int> SearchTracks()
    {
        await _searchService.SearchTracks();

        return 0;
    }
}
