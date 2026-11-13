using snowcoreBlog.Backend.UrlShortener.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.UrlShortener.Repositories.Marten;
using snowcoreBlog.Backend.UrlShortener.Steps.Create;
using snowcoreBlog.Backend.UrlShortener.Steps.Resolve;

namespace snowcoreBlog.Backend.UrlShortener.Extensions.Startup;

public static class CoreServicesConfigurationExtensions
{
    public static IServiceCollection AddCoreServicesConfiguration(this IServiceCollection services)
    {
        // Create flow
        services.AddScoped<ValidateOriginalUrlStep>();
        services.AddScoped<GenerateCodeStep>();
        services.AddScoped<PersistUrlMappingStep>();
        services.AddScoped<BuildCreateShortUrlResultStep>();

        // Resolve flow
        services.AddScoped<LoadMappingStep>();
        services.AddScoped<CheckActiveStep>();
        services.AddScoped<CheckRateLimitStep>();
        services.AddScoped<RecordClickStep>();
        services.AddScoped<BuildResolveShortUrlResultStep>();

        return services;
    }

    public static IServiceCollection AddUrlMappingRepositoriesConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IUrlMappingRepository, UrlMappingRepository>();

        return services;
    }
}
