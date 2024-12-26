using SpotiMate.Cli;
using SpotiMate.Spotify.Apis;

namespace SpotiMate.Services;

public class PlaylistService : IPlaylistService
{
    private readonly ISpotifyMeApi _spotifyMeApi;
    private readonly ISpotifyUsersApi _spotifyUsersApi;
    private readonly ISpotifyPlaylistsApi _spotifyPlaylistsApi;

    public PlaylistService(
        ISpotifyMeApi spotifyMeApi,
        ISpotifyUsersApi spotifyUsersApi,
        ISpotifyPlaylistsApi spotifyPlaylistsApi)
    {
        _spotifyMeApi = spotifyMeApi;
        _spotifyUsersApi = spotifyUsersApi;
        _spotifyPlaylistsApi = spotifyPlaylistsApi;
    }

    public async Task<string> CreatePlaylist(string playlistName)
    {
        var getProfileResponse = await _spotifyMeApi.GetCurrentUserProfile();

        if (getProfileResponse.IsError)
        {
            CliPrint.Error($"Failed to get current user profile: {getProfileResponse.Error}");
            return null;
        }

        var userId = getProfileResponse.Data.Id;

        var createPlaylistResponse = await _spotifyUsersApi.CreatePlaylist(userId, playlistName);

        if (createPlaylistResponse.IsError)
        {
            CliPrint.Error($"Failed to create playlist: {createPlaylistResponse.Error}");
            return null;
        }

        CliPrint.Success($"Successfully created playlist: {playlistName}");
        return createPlaylistResponse.Data.Id;
    }

    public async Task<bool> AddTracksToPlaylist(string playlistId, string[] trackIds)
    {
        const int chunkSize = 100;
        var chunks = trackIds.Chunk(chunkSize);

        var overallSuccess = true;

        foreach (var chunk in chunks)
        {
            var result = await _spotifyPlaylistsApi.AddTracksToPlaylist(playlistId, chunk);

            if (!result.IsError)
            {
                continue;
            }

            CliPrint.Error($"Failed to add tracks to playlist: {result.Error}");
            overallSuccess = false;
        }

        if (overallSuccess)
        {
            CliPrint.Success($"Successfully added {trackIds.Length} tracks to playlist");
        }

        return overallSuccess;
    }
}
