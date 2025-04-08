using SpotiMate.Cli;
using SpotiMate.Services;

namespace SpotiMate.Handlers;

public interface ICommandHandler
{
    Task<int> Handle(CliOptions options);
}

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
        switch (options)
        {
            case RunAllOptions runAllOptions:
                await RunAll(runAllOptions);
                break;

            case SearchTracksOptions searchTracksOptions:
                await SearchTracks(searchTracksOptions);
                break;

            default:
                throw new ArgumentException("Invalid options");
        }

        return 0;
    }

    private async Task RunAll(RunAllOptions options)
    {
        var savedTracks = await _savedTrackService.GetSavedTracks();

        if (savedTracks == null || savedTracks.Length == 0)
        {
            throw new Exception("No saved tracks found");
        }

        await _duplicateService.FindDuplicates(
            savedTracks,
            options.DuplicatesPlaylistId,
            TimeSpan.FromDays(options.Days));

        var followedArtists = await _artistService.GetFollowedArtists();

        if (followedArtists == null || followedArtists.Length == 0)
        {
            throw new Exception("No followed artists found");
        }

        await _artistService.SyncFollowedArtists(
            savedTracks,
            followedArtists,
            options.ArtistFollowersThreshold);
    }

    private async Task SearchTracks(SearchTracksOptions options)
    {
        var filePath = Path.Combine(Environment.CurrentDirectory, "Resources", "User", "playlist.txt");
        var trackNames = await File.ReadAllLinesAsync(filePath);

        var trackIds = await _searchService.SearchTracks(trackNames, options.OpenAIApiKey);

        if (trackIds == null || trackIds.Length == 0)
        {
            throw new Exception("No tracks found");
        }

        var playlistId = await _playlistService.CreatePlaylist($"YM-Playlist-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}");

        if (playlistId == null)
        {
            throw new Exception("Playlist could not be created");
        }

        await _playlistService.AddTracksToPlaylist(playlistId, trackIds);
    }
}
