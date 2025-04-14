using SpotiMate.Cli;
using SpotiMate.Spotify.Models;
using SpotiMate.Spotify.Services;

namespace SpotiMate.Services;

public interface IArtistService
{
    Task<ArtistObject[]> GetFollowedArtists();
    Task SyncFollowedArtists(
        SavedTrackObject[] savedTracks,
        ArtistObject[] followedArtists,
        int artistFollowersThreshold);
}

public class ArtistService : IArtistService
{
    private readonly ISpotifyMeService _spotifyMeService;
    private readonly ISpotifyArtistsService _spotifyArtistsService;

    public ArtistService(ISpotifyMeService spotifyMeService, ISpotifyArtistsService spotifyArtistsService)
    {
        _spotifyMeService = spotifyMeService;
        _spotifyArtistsService = spotifyArtistsService;
    }

    public async Task<ArtistObject[]> GetFollowedArtists()
    {
        CliPrint.Info("Getting followed artists");
        return await _spotifyMeService.GetFollowedArtists();
    }

    public async Task SyncFollowedArtists(
        SavedTrackObject[] savedTracks,
        ArtistObject[] followedArtists,
        int artistFollowersThreshold)
    {
        CliPrint.Info("Synchronizing followed artists");

        var savedTracksArtistIds = savedTracks
            .SelectMany(t => t.Track.Artists.Select(a => a.Id))
            .Distinct()
            .ToArray();

        await FollowArtists(
            savedTracksArtistIds,
            followedArtists,
            artistFollowersThreshold);

        await UnfollowArtists(
            savedTracksArtistIds,
            followedArtists,
            artistFollowersThreshold);
    }

    private async Task FollowArtists(
        string[] savedTracksArtistIds,
        IEnumerable<ArtistObject> followedArtists,
        int artistFollowersThreshold)
    {
        var followedArtistIds = followedArtists
            .Select(a => a.Id)
            .ToArray();

        var artistIdsToFollow = savedTracksArtistIds.Except(followedArtistIds).ToArray();

        if (artistIdsToFollow.Length == 0)
        {
            return;
        }

        var artistsToFollowInfo = await _spotifyArtistsService.GetArtists(artistIdsToFollow);

        var filteredArtistIdsToFollow = artistsToFollowInfo
            .Where(a => a.Followers.Total >= artistFollowersThreshold)
            .Select(a => a.Id)
            .ToArray();

        if (filteredArtistIdsToFollow.Length == 0)
        {
            return;
        }

        await _spotifyMeService.FollowArtists(filteredArtistIdsToFollow);

        CliPrint.Success($"Successfully followed {filteredArtistIdsToFollow.Length} artists");
    }

    private async Task UnfollowArtists(
        string[] savedTracksArtistIds,
        IEnumerable<ArtistObject> followedArtists,
        int artistFollowersThreshold)
    {
        var artistsToUnfollow = followedArtists
            .Where(a => !savedTracksArtistIds.Contains(a.Id) || a.Followers.Total < artistFollowersThreshold)
            .Select(a => a.Id)
            .ToArray();

        if (artistsToUnfollow.Length == 0)
        {
            return;
        }

        await _spotifyMeService.UnfollowArtists(artistsToUnfollow);

        CliPrint.Success($"Successfully unfollowed {artistsToUnfollow.Length} artists");
    }
}
