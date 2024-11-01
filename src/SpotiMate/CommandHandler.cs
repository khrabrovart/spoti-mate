using SpotiMate.Cli;
using SpotiMate.Services;
using SpotiMate.Spotify;
using SpotiMate.Spotify.Apis;

namespace SpotiMate;

public class CommandHandler
{
    private readonly AuthorizationService _authorizationService = new();

    public async Task<int> Handle(CliOptions options)
    {
        var accessToken = await _authorizationService.GetAccessToken(options.ClientId, options.ClientSecret, options.RefreshToken);

        return options switch
        {
            FindDuplicatesOptions findDuplicatesOptions => await FindDuplicates(findDuplicatesOptions, accessToken),
            SynchronizeArtistsOptions synchronizeArtistsOptions => await SynchronizeArtists(synchronizeArtistsOptions, accessToken),
            SearchTracksOptions searchTracksOptions => await SearchTracks(searchTracksOptions, accessToken),
            _ => throw new InvalidOperationException()
        };
    }

    private async Task<int> FindDuplicates(FindDuplicatesOptions options, string accessToken)
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

    private async Task<int> SynchronizeArtists(SynchronizeArtistsOptions options, string accessToken)
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

    private async Task<int> SearchTracks(SearchTracksOptions options, string accessToken)
    {
        var searchService = new SearchService(accessToken);

        await searchService.SearchTracks();

        return 0;
    }

    private async Task<SpotifyClient> GetSpotifyClient(CliOptions options)
    {
        var spotify = new SpotifyClient();
        await spotify.Authorize(options.ClientId, options.ClientSecret, options.RefreshToken);

        return spotify;
    }
}
