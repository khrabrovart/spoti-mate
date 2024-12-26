using SpotiMate.Cli;
using SpotiMate.Services;

namespace SpotiMate.Handlers;

public class CommandHandler : ICommandHandler
{
    private readonly ISavedTrackService _savedTrackService;
    private readonly IDuplicateService _duplicateService;
    private readonly IArtistService _artistService;
    private readonly ISearchService _searchService;
    private readonly IPlaylistService _playlistService;

    public CommandHandler(
        ISavedTrackService savedTrackService,
        IDuplicateService duplicateService,
        IArtistService artistService,
        ISearchService searchService,
        IPlaylistService playlistService)
    {
        _savedTrackService = savedTrackService;
        _duplicateService = duplicateService;
        _artistService = artistService;
        _searchService = searchService;
        _playlistService = playlistService;
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
        var savedTracks = await _savedTrackService.GetSavedTracks();

        if (savedTracks == null || savedTracks.Length == 0)
        {
            return 1;
        }

        var duplicatesProcessed = await _duplicateService.FindDuplicates(
            savedTracks,
            options.DuplicatesPlaylistId,
            TimeSpan.FromDays(options.Days));

        var artistsSynchronized = await _artistService.FollowArtists(
            savedTracks,
            TimeSpan.FromDays(options.Days));

        return duplicatesProcessed && artistsSynchronized ? 0 : 1;
    }

    private async Task<int> SearchTracks(SearchTracksOptions options)
    {
        var filePath = Path.Combine(Environment.CurrentDirectory, "Resources", "User", "playlist.txt");
        var trackNames = await File.ReadAllLinesAsync(filePath);

        var trackIds = await _searchService.SearchTracks(trackNames, options.OpenAIApiKey);

        if (trackIds == null || trackIds.Length == 0)
        {
            return 1;
        }

        var playlistId = await _playlistService.CreatePlaylist($"YM-Playlist-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}");

        if (playlistId == null)
        {
            return 1;
        }

        var success = await _playlistService.AddTracksToPlaylist(playlistId, trackIds);

        return success ? 0 : 1;
    }
}
