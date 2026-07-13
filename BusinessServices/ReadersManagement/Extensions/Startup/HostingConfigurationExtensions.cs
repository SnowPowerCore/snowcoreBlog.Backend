using System.Text.Json.Serialization;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Routing.Constraints;
using snowcoreBlog.Backend.Core.Options;
using snowcoreBlog.Backend.Email.Core.Options;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.Backend.ReadersManagement.Options;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions.Startup;

public static class HostingConfigurationExtensions
{
    public static WebApplicationBuilder UseHostingConfiguration(this WebApplicationBuilder builder)
    {
        builder.Host.UseDefaultServiceProvider(static (c, options) =>
        {
            options.ValidateScopes = true;
            options.ValidateOnBuild = true;
        });

        return builder;
    }

    public static IServiceCollection AddHostingConfiguration(this IServiceCollection services)
    {
        services.Configure<RouteOptions>(static options =>
        {
            options.SetParameterPolicy<RegexInlineRouteConstraint>("regex");
        });

        services.ConfigureHttpJsonOptions(static options =>
        {
            options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            options.SerializerOptions.SetJsonSerializationContext();
        });

        services.Configure<Argon2StringHasherOptions>(static options =>
        {
            options.Strength = Argon2HashStrength.Moderate;
        });

        services.Configure<CookiePolicyOptions>(static options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Strict;
            options.HttpOnly = HttpOnlyPolicy.Always;
            options.Secure = CookieSecurePolicy.Always;
        });

        services.Configure<ForwardedHeadersOptions>(static options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        return services;
    }

    public static IServiceCollection AddProjectOptionsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        string projectSection = "Project")
    {
        services.Configure<ProjectOptions>(configuration.GetSection(projectSection));

        return services;
    }

    public static IServiceCollection AddTokenRequirementsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        string section = "Security:ReaderAccountTokenRequirements")
    {
        services.Configure<ReaderAccountTokenRequirementOptions>(configuration.GetSection(section));

        return services;
    }

    public static IServiceCollection AddEmailSenderConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        string section = "Integrations:Email:SenderAccount")
    {
        services.Configure<SendGridSenderAccountOptions>(configuration.GetSection(section));

        return services;
    }
}