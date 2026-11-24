using Marten;
using Marten.Services;
using snowcoreBlog.Backend.Infrastructure;

namespace snowcoreBlog.Backend.UrlShortener.Extensions.Startup;

public static class MartenConfigurationExtensions
{
    private static readonly SystemTextJsonSerializer _serializer = new();

    public static IServiceCollection AddMartenConfiguration(this IServiceCollection services)
    {
        services.AddMarten(options =>
        {
            options.Policies.AllDocumentsSoftDeleted();
            _serializer.UseTypeInfoResolver(UrlShortenerSerializationContext.Default);
            options.Serializer(_serializer);
        })
            .UseLightweightSessions()
            .UseNpgsqlDataSource();

        return services;
    }
}