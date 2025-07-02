using SpotiMate.Cli;
using SpotiMate.Services;
using SpotiMate.Spotify;

namespace SpotiMate.Handlers;

public interface ICommandHandler
{
    Task<int> Handle(CliOptions options);
}

public class CommandHandler : ICommandHandler
{
    private readonly ISpotifyClient _mySpotifyClient;

    private readonly IDuplicateService _duplicateService;
    private readonly IArtistService _artistService;
    private readonly IBlendService _blendService;

    public CommandHandler(
        ISpotifyClient mySpotifyClient,
        IDuplicateService duplicateService,
        IArtistService artistService,
        IBlendService blendService)
    {
        _mySpotifyClient = mySpotifyClient;
        _duplicateService = duplicateService;
        _artistService = artistService;
        _blendService = blendService;
    }

    public async Task<int> Handle(CliOptions options)
    {
        switch (options)
        {
            case RunAllOptions runAllOptions:
                await RunAll(runAllOptions);
                break;

            case CreateBlendOptions createBlendOptions:
                await CreateBlend(createBlendOptions);
                break;

            default:
                throw new ArgumentException("Invalid options");
        }

        return 0;
    }

    private async Task RunAll(RunAllOptions options)
    {
        CliPrint.Info("Getting saved tracks");

        var savedTracks = await _mySpotifyClient.Me.GetSavedTracks();

        if (savedTracks.Length == 0)
        {
            throw new Exception("No saved tracks found");
        }

        await _duplicateService.FindDuplicates(
            savedTracks,
            options.DuplicatesPlaylistId,
            TimeSpan.FromDays(options.Days));

        await _artistService.SyncFollowedArtists(
            savedTracks,
            options.ArtistFollowersThreshold);

        CliPrint.Info("All operations completed successfully");
    }

    private async Task CreateBlend(CreateBlendOptions options)
    {
        var blendSpotifyClient = new SpotifyClient(options.ClientId, options.ClientSecret, options.BlendRefreshToken);

        await _blendService.CreateBlend(
            blendSpotifyClient,
            options.BlendAdditionalPlaylists.ToArray(),
            options.BlendSize,
            options.BlendPlaylistId);
    }
}
