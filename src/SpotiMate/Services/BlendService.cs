using SpotiMate.Cli;
using SpotiMate.Spotify;
using SpotiMate.Spotify.Models;

namespace SpotiMate.Services;

public interface IBlendService
{
    Task CreateBlend(
        ISpotifyClient otherSpotifyClient,
        int blendSize,
        string blendPlaylistId);
}

public class BlendService : IBlendService
{
    private class BlendTrack
    {
        public BlendTrack(ItemObject item, bool isMyTrack)
        {
            Item = item;
            IsMyTrack = isMyTrack;
        }

        public ItemObject Item { get; }
        public bool IsMyTrack { get; }
    }

    private class TrackComparer : IEqualityComparer<BlendTrack>
    {
        public bool Equals(BlendTrack x, BlendTrack y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.Item.Name.Equals(y.Item.Name, StringComparison.OrdinalIgnoreCase) &&
                   x.Item.Artists.Select(a => a.Id).SequenceEqual(y.Item.Artists.Select(a => a.Id));
        }

        public int GetHashCode(BlendTrack obj)
        {
            return obj.Item.Name.GetHashCode() ^
                   obj.Item.Artists.Select(a => a.Id).Aggregate(0, (hash, id) => hash ^ id.GetHashCode());
        }
    }

    private readonly ISpotifyClient _spotifyClient;

    public BlendService(ISpotifyClient spotifyClient)
    {
        _spotifyClient = spotifyClient;
    }

    public async Task CreateBlend(
        ISpotifyClient otherSpotifyClient,
        int blendSize,
        string blendPlaylistId)
    {
        CliPrint.Info("Loading saved tracks");

        var mySavedTracksTask = _spotifyClient.Me.GetSavedTracks();
        var otherSavedTracksTask = otherSpotifyClient.Me.GetSavedTracks();

        await Task.WhenAll(mySavedTracksTask, otherSavedTracksTask);

        var mySavedTracks = mySavedTracksTask.Result.Select(t => new BlendTrack(t.Item, true)).ToArray();
        var otherSavedTracks = otherSavedTracksTask.Result.Select(t => new BlendTrack(t.Item, false)).ToArray();

        if (mySavedTracks.Length == 0 || otherSavedTracks.Length == 0)
        {
            throw new Exception("No saved tracks found for one or both users");
        }

        CliPrint.Info($"Loaded {mySavedTracks.Length} saved tracks for user 1");
        CliPrint.Info($"Loaded {otherSavedTracks.Length} saved tracks for user 2");

        CliPrint.Info("Selecting tracks");

        var mySelectedTracks = Shuffle(mySavedTracks).Take(blendSize);
        var otherSelectedTracks = Shuffle(otherSavedTracks).Take(blendSize);

        CliPrint.Info("Blending");

        var allTracks = new HashSet<BlendTrack>(new TrackComparer());

        allTracks.UnionWith(otherSelectedTracks);
        allTracks.UnionWith(mySelectedTracks);

        var blendedTracks = Shuffle(allTracks).Take(blendSize);

        CliPrint.Info("Updating playlist");

        var oldBlendPlaylistTracks = await _spotifyClient.Playlists.GetPlaylistItems(blendPlaylistId);

        await _spotifyClient.Playlists.RemoveItemsFromPlaylist(
            blendPlaylistId,
            oldBlendPlaylistTracks.Select(t => t.Item.Id).ToArray());

        foreach (var track in blendedTracks)
        {
            if (track.IsMyTrack)
            {
                await _spotifyClient.Playlists.AddItemsToPlaylist(blendPlaylistId, [track.Item.Id]);
            }
            else
            {
                await otherSpotifyClient.Playlists.AddItemsToPlaylist(blendPlaylistId, [track.Item.Id]);
            }
        }

        CliPrint.Success("Blend created successfully");
    }

    private static T[] Shuffle<T>(IEnumerable<T> items)
    {
        var itemsArray = items.ToArray();

        if (itemsArray is not { Length: > 1 })
        {
            return itemsArray;
        }

        var random = new Random();

        for (var i = 0; i < itemsArray.Length - 1; i++)
        {
            var j = random.Next(i, itemsArray.Length);
            (itemsArray[i], itemsArray[j]) = (itemsArray[j], itemsArray[i]);
        }

        return itemsArray;
    }
}
