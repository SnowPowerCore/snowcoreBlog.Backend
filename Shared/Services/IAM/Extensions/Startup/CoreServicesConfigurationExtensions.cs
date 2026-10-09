using Microsoft.AspNetCore.Identity;
using snowcoreBlog.Backend.Core.Interfaces.Services;
using snowcoreBlog.Backend.Infrastructure.Services;
using snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.IAM.Repositories.Marten;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.Backend.IAM.Services;

namespace snowcoreBlog.Backend.IAM.Extensions.Startup;

public static class CoreServicesConfigurationExtensions
{
    public static IServiceCollection AddCoreServicesConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IHasher, Argon2Hasher>();
        services.AddSingleton<TotpService>();
        services.AddScoped<IApplicationTempUserRepository, ApplicationTempUserRepository>();
        services.AddScoped<IFido2PublicKeyCredentialRepository, Fido2PublicKeyCredentialRepository>();

        return services;
    }

    public static IServiceCollection AddIdentityMartenConfiguration(this IServiceCollection services)
    {
        services.AddIdentityCore<ApplicationUserEntity>()
                .AddRoles<IdentityRole>()
                .AddMartenStores<ApplicationUserEntity, IdentityRole>();

        services.AddIdentityCore<ApplicationAdminEntity>()
                .AddRoles<IdentityRole>()
                .AddMartenStores<ApplicationAdminEntity, IdentityRole>();

        return services;
    }
}