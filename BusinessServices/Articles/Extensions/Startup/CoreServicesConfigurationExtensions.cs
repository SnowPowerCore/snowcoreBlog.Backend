using snowcoreBlog.ApplicationLaunch.Implementations.BackgroundServices;
using snowcoreBlog.ApplicationLaunch.Interfaces;
using snowcoreBlog.Backend.Articles.Services;
using snowcoreBlog.Backend.Articles.Steps;
using snowcoreBlog.Backend.Articles.Steps.Articles;

namespace snowcoreBlog.Backend.Articles.Extensions.Startup;

public static class CoreServicesConfigurationExtensions
{
    public static IServiceCollection AddCoreServicesConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IApplicationLaunchService>(static sp => new ArticlesApplicationLaunchService(sp));
        services.AddScoped<ValidateAuthorAccountStep>();
        services.AddScoped<GenerateSlugStep>();
        services.AddScoped<SaveArticleStep>();
        services.AddScoped<GetArticlesCachedStep>();

        services.AddHostedService(static sp =>
            new ApplicationLaunchWorker(sp.GetRequiredService<IHostApplicationLifetime>(),
                sp.GetRequiredService<IApplicationLaunchService>()));

        return services;
    }
}