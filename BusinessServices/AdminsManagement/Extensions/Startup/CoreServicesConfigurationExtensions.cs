using snowcoreBlog.ApplicationLaunch.Implementations.BackgroundServices;
using snowcoreBlog.ApplicationLaunch.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.AdminsManagement.Options;
using snowcoreBlog.Backend.AdminsManagement.Repositories.Marten;
using snowcoreBlog.Backend.AdminsManagement.Services;
using snowcoreBlog.Backend.AdminsManagement.Steps.Admins.Delete;
using snowcoreBlog.Backend.AdminsManagement.Steps.Auth.ConfirmLogin;
using snowcoreBlog.Backend.AdminsManagement.Steps.Auth.LoginLink;
using snowcoreBlog.Backend.AdminsManagement.Steps.Invites.Accept;
using snowcoreBlog.Backend.AdminsManagement.Steps.Invites.Create;

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

        // Steps — account management
        services.AddScoped<ValidateNotDeletingSelfStep>();
        services.AddScoped<ValidateAdminExistsForDeleteStep>();
        services.AddScoped<ValidateNotLastActiveAdminStep>();
        services.AddScoped<SoftDeleteAdminStep>();

        // Steps — authentication
        services.AddScoped<ResolveActiveAdminByEmailStep>();
        services.AddScoped<CreateAdminLoginTokenStep>();
        services.AddScoped<SendAdminLoginLinkEmailStep>();
        services.AddScoped<ResolveAdminLoginTokenStep>();
        services.AddScoped<ResolveAdminForLoginStep>();
        services.AddScoped<ConsumeAdminLoginTokenStep>();
        services.AddScoped<ValidateAdminExistsInIamStep>();
        services.AddScoped<Steps.Auth.ConfirmLogin.IssueAdminSessionStep>();

        // Steps — invites
        services.AddScoped<ValidateNoAdminWithEmailStep>();
        services.AddScoped<ValidateNoOpenInviteStep>();
        services.AddScoped<CreateAdminInviteEntityStep>();
        services.AddScoped<SendAdminInviteEmailStep>();
        services.AddScoped<ResolveAdminInviteStep>();
        services.AddScoped<CreateAdminIdentityStep>();
        services.AddScoped<CreateAdminEntityStep>();
        services.AddScoped<ConsumeAdminInviteStep>();
        services.AddScoped<Steps.Invites.Accept.IssueAdminSessionStep>();

        services.AddSingleton<IApplicationLaunchService>(static sp => new AdminsApplicationLaunchService(sp));
        services.AddHostedService(static sp =>
            new ApplicationLaunchWorker(sp.GetRequiredService<IHostApplicationLifetime>(),
                sp.GetRequiredService<IApplicationLaunchService>()));

        return services;
    }

    public static IServiceCollection AddAdminRepositoriesConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IAdminInviteRepository, AdminInviteRepository>();
        services.AddScoped<IAdminLoginTokenRepository, AdminLoginTokenRepository>();

        return services;
    }
}