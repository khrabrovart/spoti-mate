using System.Collections.Concurrent;
using SpotiMate.Cli;
using SpotiMate.Spotify;
using SpotiMate.Spotify.Models;
using SpotiMate.SpotifyWeb;

namespace SpotiMate.Services;

public interface IBlendService
{
    Task CreateBlend(
        ISpotifyClient otherSpotifyClient,
        string[] additionalPlaylists,
        int blendSize,
        string blendPlaylistId);
}

public class BlendService : IBlendService
{
    private class BlendTrack
    {
        public BlendTrack(TrackObject track, bool isMyTrack)
        {
            Track = track;
            IsMyTrack = isMyTrack;
        }

        public TrackObject Track { get; }
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

            return x.Track.Name.Equals(y.Track.Name, StringComparison.OrdinalIgnoreCase) &&
                   x.Track.Artists.Select(a => a.Id).SequenceEqual(y.Track.Artists.Select(a => a.Id));
        }

        public int GetHashCode(BlendTrack obj)
        {
            return obj.Track.Name.GetHashCode() ^
                   obj.Track.Artists.Select(a => a.Id).Aggregate(0, (hash, id) => hash ^ id.GetHashCode());
        }
    }

    private readonly ISpotifyClient _spotifyClient;
    private readonly ISpotifyPlaylistWebParser _spotifyPlaylistWebParser;

    public BlendService(ISpotifyClient spotifyClient, ISpotifyPlaylistWebParser spotifyPlaylistWebParser)
    {
        _spotifyClient = spotifyClient;
        _spotifyPlaylistWebParser = spotifyPlaylistWebParser;
    }

    public async Task CreateBlend(
        ISpotifyClient otherSpotifyClient,
        string[] additionalPlaylists,
        int blendSize,
        string blendPlaylistId)
    {
        CliPrint.Info("Getting saved tracks");

        var mySavedTracksTask = _spotifyClient.Me.GetSavedTracks();
        var otherSavedTracksTask = otherSpotifyClient.Me.GetSavedTracks();

        await Task.WhenAll(mySavedTracksTask, otherSavedTracksTask);

        var mySavedTracks = mySavedTracksTask.Result.Select(t => new BlendTrack(t.Track, true)).ToArray();
        var otherSavedTracks = otherSavedTracksTask.Result.Select(t => new BlendTrack(t.Track, false)).ToArray();

        if (mySavedTracks.Length == 0 || otherSavedTracks.Length == 0)
        {
            throw new Exception("No saved tracks found for one or both users");
        }

        var commonTracks = mySavedTracks
            .Intersect(otherSavedTracks, new TrackComparer())
            .Select((t, i) => new BlendTrack(t.Track, i % 2 == 0))
            .ToArray();

        CliPrint.Info("Loading additional playlists");

        var additionalTracksList = new ConcurrentBag<TrackObject>();

        var additionalTasks = additionalPlaylists
            .Select(async (playlistId, i) =>
            {
                CliPrint.Info($"Loading additional playlist tracks: {i}");
                await LoadAdditionalPlaylistTracks(playlistId, additionalTracksList);
                CliPrint.Info($"Additional playlist tracks loaded: {i}");
            });

        await Task.WhenAll(additionalTasks);

        var additionalTracks = additionalTracksList
            .Select((t, i) => new BlendTrack(t, i % 2 == 0))
            .Distinct(new TrackComparer())
            .ToArray();

        CliPrint.Info("Selecting tracks");

        var mySelectedTracks = Shuffle(mySavedTracks).Take(blendSize);
        var otherSelectedTracks = Shuffle(otherSavedTracks).Take(blendSize);
        var additionalSelectedTracks = Shuffle(additionalTracks).Take(blendSize);
        var commonSelectedTracks = Shuffle(commonTracks).Take(blendSize);

        CliPrint.Info("Blending");

        var allTracks = new HashSet<BlendTrack>(new TrackComparer());

        allTracks.UnionWith(commonSelectedTracks);
        allTracks.UnionWith(otherSelectedTracks);
        allTracks.UnionWith(mySelectedTracks);
        allTracks.UnionWith(additionalSelectedTracks);

        var blendedTracks = Shuffle(allTracks).Take(blendSize);

        CliPrint.Info("Updating playlist");

        var oldBlendPlaylistTracks = await _spotifyClient.Playlists.GetPlaylistTracks(blendPlaylistId);

        await _spotifyClient.Playlists.RemoveTracksFromPlaylist(
            blendPlaylistId,
            oldBlendPlaylistTracks.Select(t => t.Track.Id).ToArray());

        foreach (var track in blendedTracks)
        {
            if (track.IsMyTrack)
            {
                await _spotifyClient.Playlists.AddTracksToPlaylist(blendPlaylistId, [track.Track.Id]);
            }
            else
            {
                await otherSpotifyClient.Playlists.AddTracksToPlaylist(blendPlaylistId, [track.Track.Id]);
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

    private async Task LoadAdditionalPlaylistTracks(string playlistId, ConcurrentBag<TrackObject> additionalTracks)
    {
        var tracksIds = await _spotifyPlaylistWebParser.GetTrackIds(playlistId);
        var tracks = await _spotifyClient.Tracks.GetTracks(tracksIds);

        foreach (var track in tracks)
        {
            additionalTracks.Add(track);
        }
    }
}
