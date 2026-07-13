using snowcoreBlog.ApplicationLaunch.Implementations.BackgroundServices;
using snowcoreBlog.ApplicationLaunch.Interfaces;
using snowcoreBlog.Backend.AuthorsManagement.Services;
using snowcoreBlog.Backend.AuthorsManagement.Steps;
using snowcoreBlog.Backend.Infrastructure.Middleware;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.AuthorsManagement.Extensions.Startup;

public static class CoreServicesConfigurationExtensions
{
    public static IServiceCollection AddCoreServicesConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IApplicationLaunchService>(static sp =>
            new AuthorsManagementApplicationLaunchService(sp.GetRequiredService<IConnectionMultiplexer>()));
        services.AddScoped<UserCookieJsonWebTokenMiddleware>();
        services.AddScoped<CreateAuthorEntityForExistingUserStep>();

        services.AddHostedService(static sp =>
            new ApplicationLaunchWorker(sp.GetRequiredService<IHostApplicationLifetime>(),
                sp.GetRequiredService<IApplicationLaunchService>()));

        return services;
    }
}