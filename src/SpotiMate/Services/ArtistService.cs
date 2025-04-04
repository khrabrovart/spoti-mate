using System.Diagnostics;
using SpotiMate.Cli;
using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Models;

namespace SpotiMate.Services;

public class ArtistService : IArtistService
{
    private readonly ISpotifyMeApi _spotifyMeApi;

    public ArtistService(ISpotifyMeApi spotifyMeApi)
    {
        _spotifyMeApi = spotifyMeApi;
    }

    public async Task<bool> SyncArtists(IEnumerable<SavedTrackObject> savedTracks)
    {
        CliPrint.Info("Synchronizing artists");

        var uniqueArtistIds = savedTracks
            .SelectMany(t => t.Track.Artists.Select(a => a.Id))
            .Distinct()
            .ToArray();

        var followedArtists = await GetFollowedArtists();

        if (followedArtists == null)
        {
            return false;
        }

        var followedArtistsIds = followedArtists
            .Select(a => a.Id)
            .ToArray();

        var artistsToFollow = uniqueArtistIds.Except(followedArtistsIds).ToArray();
        var artistsToUnfollow = followedArtistsIds.Except(uniqueArtistIds).ToArray();

        if (artistsToFollow.Length == 0 && artistsToUnfollow.Length == 0)
        {
            CliPrint.Success("No artists to synchronize");
            return true;
        }

        var overallSuccess = true;

        if (artistsToFollow.Length > 0)
        {
            if (!await FollowArtists(artistsToFollow))
            {
                overallSuccess = false;
            }
        }

        if (artistsToUnfollow.Length > 0)
        {
            if (!await UnfollowArtists(artistsToUnfollow))
            {
                overallSuccess = false;
            }
        }

        if (overallSuccess)
        {
            CliPrint.Success("Successfully synchronized artists");
        }

        return overallSuccess;
    }

    private async Task<bool> FollowArtists(string[] artistIds)
    {
        CliPrint.Info($"Following {artistIds.Length} artists");

        const int chunkSize = 50;
        var success = true;
        var chunks = artistIds.Chunk(chunkSize);

        foreach (var chunk in chunks)
        {
            var result = await _spotifyMeApi.FollowArtists(chunk);

            if (!result.IsError)
            {
                continue;
            }

            CliPrint.Error($"Failed to follow artists: {result.Error}");
            success = false;
        }

        return success;
    }

    private async Task<bool> UnfollowArtists(string[] artistIds)
    {
        CliPrint.Info($"Unfollowing {artistIds.Length} artists");

        const int chunkSize = 50;
        var success = true;
        var chunks = artistIds.Chunk(chunkSize);

        foreach (var chunk in chunks)
        {
            var result = await _spotifyMeApi.UnfollowArtists(chunk);

            if (!result.IsError)
            {
                continue;
            }

            CliPrint.Error($"Failed to unfollow artists: {result.Error}");
            success = false;
        }

        return success;
    }

    private async Task<ArtistObject[]> GetFollowedArtists()
    {
        CliPrint.Info("Loading followed artists");

        var artists = new List<ArtistObject>();

        string lastArtistId = null;
        const int limit = 50;

        var sw = Stopwatch.StartNew();

        while (true)
        {
            var response = await _spotifyMeApi.GetFollowedArtists(lastArtistId, limit);

            if (response.IsError)
            {
                CliPrint.Error($"Failed to load followed artists: {response.Error}");
                return null;
            }

            artists.AddRange(response.Data.Artists.Items);

            if (artists.Count >= response.Data.Artists.Total)
            {
                break;
            }

            lastArtistId = response.Data.Artists.Items.LastOrDefault()?.Id;
        }

        CliPrint.Success($"Loaded {artists.Count} followed artists in {sw.Elapsed.TotalSeconds:F2}s");

        return artists.ToArray();
    }
}
