using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Models;

namespace SpotiMate.Spotify.Services;

public interface ISpotifyMeService : ISpotifyService<ISpotifyMeApi>
{
    Task<UserProfile> GetCurrentUserProfile();
    Task<SavedTrackObject[]> GetSavedTracks();
    Task<ArtistObject[]> GetFollowedArtists();
    Task FollowArtists(string[] artistIds);
    Task UnfollowArtists(string[] artistIds);
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
        string lastArtistId = null;

        while (true)
        {
            var response = await Api.GetFollowedArtists(lastArtistId, limit);

            if (response.IsError)
            {
                throw new Exception($"Failed to get followed artists: {response.Error}");
            }

            artists.AddRange(response.Data.Artists.Items);

            if (artists.Count >= response.Data.Artists.Total)
            {
                return artists.ToArray();
            }

            lastArtistId = response.Data.Artists.Items.LastOrDefault()?.Id;
        }
    }

    public async Task FollowArtists(string[] artistIds)
    {
        const int chunkSize = 50;
        var chunks = artistIds.Chunk(chunkSize);

        foreach (var chunk in chunks)
        {
            var result = await Api.FollowArtists(chunk);

            if (result.IsError)
            {
                throw new Exception($"Failed to follow artists: {result.Error}");
            }
        }
    }

    public async Task UnfollowArtists(string[] artistIds)
    {
        const int chunkSize = 50;
        var chunks = artistIds.Chunk(chunkSize);

        foreach (var chunk in chunks)
        {
            var result = await Api.UnfollowArtists(chunk);

            if (result.IsError)
            {
                throw new Exception($"Failed to unfollow artists: {result.Error}");
            }
        }
    }
}
