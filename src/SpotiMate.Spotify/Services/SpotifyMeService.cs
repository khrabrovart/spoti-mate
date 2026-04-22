using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Models.Requests;
using SpotiMate.Spotify.Models.Responses;

namespace SpotiMate.Spotify.Services;

public interface ISpotifyMeService : ISpotifyService<ISpotifyMeApi>
{
    Task<UserProfile> GetCurrentUserProfile();
    Task<SavedTrackObject[]> GetSavedTracks();
    Task<ArtistObject[]> GetFollowedArtists();
    Task SaveLibraryItems(SpotifyLibraryItemType itemType, string[] ids);
    Task RemoveLibraryItems(SpotifyLibraryItemType itemType, string[] ids);
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

    public async Task<ArtistObject[]> GetFollowedArtists()
    {
        var artists = new List<ArtistObject>();

        const int limit = 50;
        string after = null;

        while (true)
        {
            var page = await Api.GetFollowedArtists(after, limit);

            if (page.IsError)
            {
                throw new Exception($"Failed to get followed artists: {page.Error}");
            }

            artists.AddRange(page.Data.Artists.Items);
            after = page.Data.Artists.Cursors.After;

            if (string.IsNullOrEmpty(after))
            {
                return [.. artists];
            }
        }
    }

    public async Task SaveLibraryItems(SpotifyLibraryItemType itemType, string[] ids)
    {
        const int chunkSize = 40;

        foreach (var chunk in ids.Chunk(chunkSize))
        {
            var response = await Api.SaveLibraryItems(itemType, chunk.ToArray());

            if (response.IsError)
            {
                throw new Exception($"Failed to save library items: {response.Error}");
            }
        }
    }

    public async Task RemoveLibraryItems(SpotifyLibraryItemType itemType, string[] ids)
    {
        const int chunkSize = 40;

        foreach (var chunk in ids.Chunk(chunkSize))
        {
            var response = await Api.RemoveLibraryItems(itemType, chunk.ToArray());

            if (response.IsError)
            {
                throw new Exception($"Failed to remove library items: {response.Error}");
            }
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
