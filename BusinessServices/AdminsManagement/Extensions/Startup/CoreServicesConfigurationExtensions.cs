using snowcoreBlog.ApplicationLaunch.Implementations.BackgroundServices;
using snowcoreBlog.ApplicationLaunch.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Options;
using snowcoreBlog.Backend.AdminsManagement.Services;

namespace snowcoreBlog.Backend.AdminsManagement.Extensions.Startup;

public static class CoreServicesConfigurationExtensions
{
    public static IServiceCollection AddCoreServicesConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AdminsManagementOptions>(configuration.GetSection("AdminsManagement"));
        services.Configure<AdminEmailSenderOptions>(configuration.GetSection("Integrations:Email:SenderAccount"));
        services.Configure<AdminIssuerSigningKeyOptions>(options =>
            options.Value = configuration["Security:Signing:Admin:SigningKey"]
                ?? throw new InvalidOperationException("Admin signing key not configured"));

        services.AddHttpContextAccessor();
        services.AddScoped<AdminTokenPairIssuer>();
        services.AddScoped<AdminEmailPublisher>();

        services.AddSingleton<IApplicationLaunchService>(static sp => new AdminsApplicationLaunchService(sp));
        services.AddHostedService(static sp =>
            new ApplicationLaunchWorker(sp.GetRequiredService<IHostApplicationLifetime>(),
                sp.GetRequiredService<IApplicationLaunchService>()));

        return services;
    }
}