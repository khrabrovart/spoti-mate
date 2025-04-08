using SpotiMate.Cli;
using SpotiMate.Spotify.Models;
using SpotiMate.Spotify.Services;

namespace SpotiMate.Services;

public interface ISavedTrackService
{
    Task<SavedTrackObject[]> GetSavedTracks();
}

public class SavedTrackService : ISavedTrackService
{
    private readonly ISpotifyMeService _spotifyMeService;

    public SavedTrackService(ISpotifyMeService spotifyMeService)
    {
        _spotifyMeService = spotifyMeService;
    }

    public async Task<SavedTrackObject[]> GetSavedTracks()
    {
        CliPrint.Info("Getting saved tracks");
        return await _spotifyMeService.GetSavedTracks();
    }
}
