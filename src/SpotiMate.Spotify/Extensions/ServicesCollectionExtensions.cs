using Microsoft.Extensions.DependencyInjection;
using SpotiMate.Spotify.Apis;
using SpotiMate.Spotify.Providers;

namespace SpotiMate.Spotify.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddSpotify(this IServiceCollection services, string clientId, string clientSecret, string refreshToken)
    {
        services
            .AddTransient<ISpotifyMeApi, SpotifyMeApi>()
            .AddTransient<ISpotifyPlaylistsApi, SpotifyPlaylistsApi>()
            .AddTransient<ISpotifySearchApi, SpotifySearchApi>()
            .AddTransient<ISpotifyUsersApi, SpotifyUsersApi>()
            .AddSingleton<ISpotifyAuthProvider>(_ => new SpotifyAuthProvider(clientId, clientSecret, refreshToken));

        return services;
    }
}
