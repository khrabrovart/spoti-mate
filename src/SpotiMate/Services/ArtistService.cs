using SpotiMate.Cli;
using SpotiMate.Spotify;
using SpotiMate.Spotify.Models;

namespace SpotiMate.Services;

public interface IArtistService
{
    Task SyncFollowedArtists(
        SavedTrackObject[] savedTracks,
        int artistFollowersThreshold);
}

public class ArtistService : IArtistService
{
    private readonly ISpotifyClient _spotifyClient;

    public ArtistService(ISpotifyClient spotifyClient)
    {
        _spotifyClient = spotifyClient;
    }

    public async Task SyncFollowedArtists(
        SavedTrackObject[] savedTracks,
        int artistFollowersThreshold)
    {
        CliPrint.Info("Getting followed artists");

        var followedArtists = await _spotifyClient.Me.GetFollowedArtists();

        if (followedArtists == null || followedArtists.Length == 0)
        {
            throw new Exception("No followed artists found");
        }

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

        var artistsToFollowInfo = await _spotifyClient.Artists.GetArtists(artistIdsToFollow);

        var filteredArtistIdsToFollow = artistsToFollowInfo
            .Where(a => a.Followers.Total >= artistFollowersThreshold)
            .Select(a => a.Id)
            .ToArray();

        if (filteredArtistIdsToFollow.Length == 0)
        {
            return;
        }

        await _spotifyClient.Me.FollowArtists(filteredArtistIdsToFollow);

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

        await _spotifyClient.Me.UnfollowArtists(artistsToUnfollow);

        CliPrint.Success($"Successfully unfollowed {artistsToUnfollow.Length} artists");
    }
}
