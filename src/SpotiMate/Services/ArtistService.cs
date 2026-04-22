using SpotiMate.Cli;
using SpotiMate.Spotify;
using SpotiMate.Spotify.Models.Requests;

namespace SpotiMate.Services;

public interface IArtistService
{
    Task FollowAllFromSavedTracks();
    Task UnfollowAllFollowed();
}

public class ArtistService : IArtistService
{
    private readonly ISpotifyClient _spotifyClient;

    public ArtistService(ISpotifyClient spotifyClient)
    {
        _spotifyClient = spotifyClient;
    }

    public async Task FollowAllFromSavedTracks()
    {
        CliPrint.Info("Loading saved tracks");

        var savedTracks = await _spotifyClient.Me.GetSavedTracks();

        if (savedTracks.Length == 0)
        {
            CliPrint.Info("No saved tracks found");
            return;
        }

        var uniqueArtistIds = savedTracks
            .SelectMany(t => t.Item.Artists)
            .Select(a => a.Id)
            .Distinct()
            .ToArray();

        CliPrint.Info($"Following {uniqueArtistIds.Length} unique artists");

        await _spotifyClient.Me.SaveLibraryItems(SpotifyLibraryItemType.Artist, uniqueArtistIds);

        CliPrint.Success("Following artists completed");
    }

    public async Task UnfollowAllFollowed()
    {
        CliPrint.Info("Loading followed artists");

        var artists = await _spotifyClient.Me.GetFollowedArtists();

        if (artists.Length == 0)
        {
            CliPrint.Info("No followed artists found");
            return;
        }

        var ids = artists.Select(a => a.Id).ToArray();

        CliPrint.Info($"Unfollowing {ids.Length} artists");

        await _spotifyClient.Me.RemoveLibraryItems(SpotifyLibraryItemType.Artist, ids);

        CliPrint.Success("Unfollowing artists completed");
    }
}
