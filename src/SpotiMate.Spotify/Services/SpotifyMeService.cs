using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Models;

namespace SpotiMate.Spotify.Services;

public interface ISpotifyMeService : ISpotifyService<ISpotifyMeApi>
{
    Task<UserProfile> GetCurrentUserProfile();
    Task<SavedTrackObject[]> GetSavedTracks();
    Task<Playlist> CreatePlaylist(string playlistName);
}

public class SpotifyMeService : ISpotifyMeService
{
    public SpotifyMeService(ISpotifyMeApi spotifyMeApi)
    {
        Api = spotifyMeApi;
    }

    public ISpotifyMeApi Api { get; }

    public async Task<UserProfile> GetCurrentUserProfile()
    {
        var response = await Api.GetCurrentUserProfile();

        if (response.IsError)
        {
            throw new Exception($"Failed to get user profile: {response.Error}");
        }

        return response.Data;
    }

    public async Task<SavedTrackObject[]> GetSavedTracks()
    {
        var tracks = new List<SavedTrackObject>();

        const int limit = 50;
        var offset = 0;

        while (true)
        {
            var page = await Api.GetSavedTracks(offset, limit);

            if (page.IsError)
            {
                throw new Exception($"Failed to get saved tracks: {page.Error}");
            }

            tracks.AddRange(page.Data.Items);

            if (tracks.Count >= page.Data.Total)
            {
                return tracks.ToArray();
            }

            offset += limit;
        }
    }

    public async Task<Playlist> CreatePlaylist(string playlistName)
    {
        var response = await Api.CreatePlaylist(playlistName);

        if (response.IsError)
        {
            throw new Exception($"Failed to create playlist: {response.Error}");
        }

        return response.Data;
    }
}
