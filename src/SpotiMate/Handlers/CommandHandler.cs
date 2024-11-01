using SpotiMate.Cli;
using SpotiMate.Services;
using SpotiMate.Spotify;

namespace SpotiMate.Handlers;

public class CommandHandler : ICommandHandler
{
    private readonly ISearchService _searchService;

    public CommandHandler(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task<int> Handle(CliOptions options)
    {
        return options switch
        {
            FindDuplicatesOptions findDuplicatesOptions => await FindDuplicates(findDuplicatesOptions),
            SynchronizeArtistsOptions synchronizeArtistsOptions => await SynchronizeArtists(synchronizeArtistsOptions),
            SearchTracksOptions searchTracksOptions => await SearchTracks(searchTracksOptions),
            _ => throw new InvalidOperationException()
        };
    }

    private async Task<int> FindDuplicates(FindDuplicatesOptions options)
    {
        var spotify = await GetSpotifyClient(options);

        CliPrint.PrintInfo($"Finding duplicates for the last {options.Days} days...");

        var savedTracks = await new SavedTracksService().GetSavedTracks(spotify);

        if (savedTracks == null || savedTracks.Length == 0)
        {
            return 1;
        }

        var result = await new DuplicatesService().FindDuplicates(
            spotify,
            savedTracks,
            options.DuplicatesPlaylistId,
            TimeSpan.FromDays(options.Days));

        return result ? 0 : 1;
    }

    private async Task<int> SynchronizeArtists(SynchronizeArtistsOptions options)
    {
        var spotify = await GetSpotifyClient(options);

        CliPrint.PrintInfo($"Synchronizing artists for the last {options.Days} days...");

        var savedTracks = await new SavedTracksService().GetSavedTracks(spotify);

        if (savedTracks == null || savedTracks.Length == 0)
        {
            return 1;
        }

        var result = await new ArtistsService().SynchronizeArtists(
            spotify,
            savedTracks,
            TimeSpan.FromDays(options.Days));

        return result ? 0 : 1;
    }

    private async Task<int> SearchTracks(SearchTracksOptions options)
    {
        await _searchService.SearchTracks();

        return 0;
    }

    private async Task<SpotifyClient> GetSpotifyClient(CliOptions options)
    {
        var spotify = new SpotifyClient();
        await spotify.Authorize(options.ClientId, options.ClientSecret, options.RefreshToken);

        return spotify;
    }
}
