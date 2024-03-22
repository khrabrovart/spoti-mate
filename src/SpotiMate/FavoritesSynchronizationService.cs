using SpotiMate.Cli;
using SpotiMate.Spotify;

namespace SpotiMate;

public class FavoritesSynchronizationService
{
    public async Task SynchronizeFavorites(SpotifyClient spotify, string favoritesPlaylistId)
    {
        CliPrint.PrintInfo("Getting saved tracks...");
        var savedTracks = await spotify.GetSavedTracks();
        CliPrint.PrintSuccess($"Found {savedTracks.Count} saved tracks.");
        
        CliPrint.PrintInfo("Getting favorite tracks...");
        var favoriteTracks = await spotify.GetPlaylistTracks(favoritesPlaylistId);
        CliPrint.PrintSuccess($"Found {favoriteTracks.Count} favorite tracks.");
        
        var savedTrackIds = savedTracks.Select(t => t.Track.Id).ToHashSet();
        var favoriteTrackIds = favoriteTracks.Select(t => t.Track.Id).ToHashSet();
        
        var newTracks = savedTrackIds.Except(favoriteTrackIds).ToArray();
        var removedTracks = favoriteTrackIds.Except(savedTrackIds).ToArray();
        
        if (newTracks.Length == 0 && removedTracks.Length == 0)
        {
            CliPrint.PrintSuccess("No changes detected.");
            return;
        }

        if (newTracks.Length > 0)
        {
            await AddTracksToPlaylist(spotify, favoritesPlaylistId, newTracks);
        }

        if (removedTracks.Length > 0)
        {
            await RemoveTracksFromPlaylist(spotify, favoritesPlaylistId, removedTracks);
        }
    }
    
    private static async Task AddTracksToPlaylist(SpotifyClient spotify, string playlistId, string[] trackIds)
    {
        CliPrint.PrintInfo($"Adding {trackIds.Length} tracks to favorites...");
        var added = await spotify.AddTracksToPlaylist(playlistId, trackIds);
        
        if (!added)
        {
            CliPrint.PrintError("Failed to add tracks to favorites.");
        }
        else
        {
            CliPrint.PrintSuccess("Tracks added to favorites.");
        }
    }
    
    private static async Task RemoveTracksFromPlaylist(SpotifyClient spotify, string playlistId, string[] trackIds)
    {
        CliPrint.PrintInfo($"Removing {trackIds.Length} tracks from favorites...");
        var removed = await spotify.RemoveTracksFromPlaylist(playlistId, trackIds);
        
        if (!removed)
        {
            CliPrint.PrintError("Failed to remove tracks from favorites.");
        }
        else
        {
            CliPrint.PrintSuccess("Tracks removed from favorites.");
        }
    }
}