using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Models;

namespace SpotiMate.Spotify.Services;

public interface ISpotifyTracksService : ISpotifyService<ISpotifyTracksApi>
{
    Task<TrackObject[]> GetTracks(string[] trackIds);
}

public class SpotifyTracksService : ISpotifyTracksService
{
    public SpotifyTracksService(ISpotifyTracksApi spotifyTracksApi)
    {
        Api = spotifyTracksApi;
    }

    public ISpotifyTracksApi Api { get; }

    public async Task<TrackObject[]> GetTracks(string[] trackIds)
    {
        const int chunkSize = 50;
        var chunks = trackIds.Chunk(chunkSize);
        var tracks = new List<TrackObject>();

        foreach (var chunk in chunks)
        {
            var response = await Api.GetTracks(chunk);

            if (response.IsError)
            {
                throw new Exception($"Failed to get tracks: {response.Error}");
            }

            tracks.AddRange(response.Data.Tracks);
        }

        return tracks.ToArray();
    }
}
