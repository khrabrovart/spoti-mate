using SpotiMate.Spotify.Models;
using SpotiMate.Spotify.Providers;

namespace SpotiMate.Spotify.Apis;

public interface ISpotifyArtistsApi
{
    Task<ApiResponse<GetArtistsResponse>> GetArtists(string[] artistIds);
    Task<ApiResponse<ArtistTopTracks>> GetArtistTopTracks(string artistId);
}

public class SpotifyArtistsApi : SpotifyApiBase, ISpotifyArtistsApi
{
    public SpotifyArtistsApi(ISpotifyAuthProvider authProvider) : base(authProvider, "artists")
    {
    }

    public async Task<ApiResponse<GetArtistsResponse>> GetArtists(string[] artistIds)
    {
        FieldValidator.Length(nameof(artistIds), artistIds, min: 0, max: 50);

        var queryParams = new Dictionary<string, string>
        {
            { "ids", string.Join(",", artistIds) }
        };

        return await MakeRequest<GetArtistsResponse>(
            HttpMethod.Get,
            queryParams: queryParams);
    }

    public async Task<ApiResponse<ArtistTopTracks>> GetArtistTopTracks(string artistId)
    {
        return await MakeRequest<ArtistTopTracks>(
            HttpMethod.Get,
            segment: $"{artistId}/top-tracks");
    }
}
