using SpotiMate.Spotify.Models;
using SpotiMate.Spotify.Providers;

namespace SpotiMate.Spotify.Apis;

public interface ISpotifyTracksApi
{
    Task<ApiResponse<GetTracksResponse>> GetTracks(string[] trackIds);
}

public class SpotifyTracksApi : SpotifyApiBase, ISpotifyTracksApi
{
    public SpotifyTracksApi(ISpotifyAuthProvider authProvider) : base(authProvider, "tracks")
    {
    }

    public async Task<ApiResponse<GetTracksResponse>> GetTracks(string[] trackIds)
    {
        FieldValidator.Length(nameof(trackIds), trackIds, min: 0, max: 50);

        var queryParams = new Dictionary<string, string>
        {
            { "ids", string.Join(",", trackIds) }
        };

        return await MakeRequest<GetTracksResponse>(
            HttpMethod.Get,
            queryParams: queryParams);
    }
}
