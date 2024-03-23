using SpotiMate.Cli;
using SpotiMate.Spotify;
using SpotiMate.Spotify.Objects;

namespace SpotiMate;

public class FavoritesSynchronizationService
{
    public async Task<bool> SynchronizeFavorites(
        SpotifyClient spotify, 
        IEnumerable<SavedTrackObject> savedTracks, 
        string favoritesPlaylistId)
    {
        CliPrint.PrintInfo("Loading favorite tracks...");
        var favoriteTracks = await spotify.GetPlaylistTracks(favoritesPlaylistId);
        CliPrint.PrintSuccess($"Found {favoriteTracks.Count} favorite tracks.");
        
        var savedTrackIds = savedTracks.Select(t => t.Track.Id).ToHashSet();
        var favoriteTrackIds = favoriteTracks.Select(t => t.Track.Id).ToHashSet();
        
        var newTracks = savedTrackIds.Except(favoriteTrackIds).ToArray();
        var removedTracks = favoriteTrackIds.Except(savedTrackIds).ToArray();
        
        if (newTracks.Length == 0 && removedTracks.Length == 0)
        {
            CliPrint.PrintSuccess("No changes detected.");
            return true;
        }

        var success = true;

        if (newTracks.Length > 0)
        {
            success &= await AddTracksToPlaylist(spotify, favoritesPlaylistId, newTracks);
        }

        if (removedTracks.Length > 0)
        {
            success &= await RemoveTracksFromPlaylist(spotify, favoritesPlaylistId, removedTracks);
        }

        return success;
    }
    
    private static async Task<bool> AddTracksToPlaylist(SpotifyClient spotify, string playlistId, string[] trackIds)
    {
        CliPrint.PrintInfo($"Adding {trackIds.Length} tracks to favorites...");
        var added = await spotify.AddTracksToPlaylist(playlistId, trackIds);
        
        if (!added)
        {
            CliPrint.PrintError("Failed to add tracks to favorites.");
            return false;
        }

        CliPrint.PrintSuccess("Tracks added to favorites.");
        return true;
    }
    
    private static async Task<bool> RemoveTracksFromPlaylist(SpotifyClient spotify, string playlistId, string[] trackIds)
    {
        CliPrint.PrintInfo($"Removing {trackIds.Length} tracks from favorites...");
        var removed = await spotify.RemoveTracksFromPlaylist(playlistId, trackIds);
        
        if (!removed)
        {
            CliPrint.PrintError("Failed to remove tracks from favorites.");
            return false;
        }

        CliPrint.PrintSuccess("Tracks removed from favorites.");
        return true;
    }
}