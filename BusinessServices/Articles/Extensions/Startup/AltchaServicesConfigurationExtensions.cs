using Ixnas.AltchaNet;
using snowcoreBlog.Backend.Infrastructure.Stores;

namespace snowcoreBlog.Backend.Articles.Extensions.Startup;

public static class AltchaServicesConfigurationExtensions
{
    public static IServiceCollection AddAltchaServicesConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IAltchaCancellableChallengeStore, AltchaChallengeStore>();

        return services;
    }
}