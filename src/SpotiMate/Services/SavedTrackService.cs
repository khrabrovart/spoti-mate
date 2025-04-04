using System.Diagnostics;
using SpotiMate.Cli;
using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Models;

namespace SpotiMate.Services;

public class SavedTrackService : ISavedTrackService
{
    private readonly ISpotifyMeApi _spotifyMeApi;

    public SavedTrackService(ISpotifyMeApi spotifyMeApi)
    {
        _spotifyMeApi = spotifyMeApi;
    }

    public async Task<SavedTrackObject[]> GetSavedTracks()
    {
        CliPrint.Info("Loading saved tracks");

        var savedTracks = new List<SavedTrackObject>();

        var offset = 0;
        const int limit = 50;

        var sw = Stopwatch.StartNew();

        while (true)
        {
            var page = await _spotifyMeApi.GetSavedTracks(offset, limit);

            if (page.IsError)
            {
                CliPrint.Error($"Failed to load saved tracks: {page.Error}");
                return null;
            }

            savedTracks.AddRange(page.Data.Items);

            if (savedTracks.Count >= page.Data.Total)
            {
                break;
            }

            offset += limit;
        }

        CliPrint.Success($"Loaded {savedTracks.Count} saved tracks in {sw.Elapsed.TotalSeconds:F2}s");

        return savedTracks.ToArray();
    }
}
