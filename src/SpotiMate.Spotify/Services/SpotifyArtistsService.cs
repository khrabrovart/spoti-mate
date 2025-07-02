using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Models;

namespace SpotiMate.Spotify.Services;

public interface ISpotifyArtistsService : ISpotifyService<ISpotifyArtistsApi>
{
    Task<ArtistObject[]> GetArtists(string[] artistIds);
    Task<TrackObject[]> GetArtistTopTracks(string artistId);
}

public class SpotifyArtistsService : ISpotifyArtistsService
{
    public SpotifyArtistsService(ISpotifyArtistsApi spotifyArtistsApi)
    {
        Api = spotifyArtistsApi;
    }

    public ISpotifyArtistsApi Api { get; }

    public async Task<ArtistObject[]> GetArtists(string [] artistIds)
    {
        const int chunkSize = 50;
        var chunks = artistIds.Chunk(chunkSize);
        var artists = new List<ArtistObject>();

        foreach (var chunk in chunks)
        {
            var response = await Api.GetArtists(chunk);

            if (response.IsError)
            {
                throw new Exception($"Failed to get artists: {response.Error}");
            }

            artists.AddRange(response.Data.Artists);
        }

        return artists.ToArray();
    }

    public async Task<TrackObject[]> GetArtistTopTracks(string artistId)
    {
        var response = await Api.GetArtistTopTracks(artistId);

        if (response.IsError)
        {
            throw new Exception($"Failed to get artist top tracks: {response.Error}");
        }

        return response.Data.Tracks;
    }
}
