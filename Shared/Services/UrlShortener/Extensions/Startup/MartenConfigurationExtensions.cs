using Marten;
using Marten.Services;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.UrlShortener.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.UrlShortener.Repositories.Marten;

namespace snowcoreBlog.Backend.UrlShortener.Extensions.Startup;

public static class MartenConfigurationExtensions
{
    private static readonly SystemTextJsonSerializer _serializer = new();

    public static IServiceCollection AddMartenConfiguration(this IServiceCollection services)
    {
        services.AddMarten(options =>
        {
            options.Policies.AllDocumentsSoftDeleted();
            _serializer.UseTypeInfoResolver(CoreSerializationContext.Default);
            options.Serializer(_serializer);
        })
            .UseLightweightSessions()
            .UseNpgsqlDataSource();

        services.AddScoped<IUrlMappingRepository, UrlMappingRepository>();

        return services;
    }
}