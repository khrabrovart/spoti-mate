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
    private readonly IBlendService _blendService;

    public CommandHandler(
        ISpotifyClient mySpotifyClient,
        IDuplicateService duplicateService,
        IBlendService blendService)
    {
        _mySpotifyClient = mySpotifyClient;
        _duplicateService = duplicateService;
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
        CliPrint.Info("Loading saved tracks");

        var savedTracks = await _mySpotifyClient.Me.GetSavedTracks();

        if (savedTracks.Length == 0)
        {
            throw new Exception("No saved tracks found");
        }

        CliPrint.Info($"Loaded {savedTracks.Length} saved tracks");

        await _duplicateService.FindDuplicates(
            savedTracks,
            options.DuplicatesPlaylistId,
            TimeSpan.FromDays(options.Days));

        CliPrint.Success("All operations completed successfully");
    }

    private async Task CreateBlend(CreateBlendOptions options)
    {
        var blendSpotifyClient = new SpotifyClient(options.ClientId, options.ClientSecret, options.BlendRefreshToken);

        await _blendService.CreateBlend(
            blendSpotifyClient,
            options.BlendSize,
            options.BlendPlaylistId);
    }
}
