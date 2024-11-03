using Microsoft.Extensions.DependencyInjection;
using SpotiMate.OpenAI.Services;

namespace SpotiMate.OpenAI.Extensions;

public static class ServicesCollectionExtensions
{
    public static IServiceCollection AddOpenAI(this IServiceCollection services)
    {
        services
            .AddTransient<IOpenAIChatService, OpenAIChatService>();

        return services;
    }
}
