using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using snowcoreBlog.PublicApi.Extensions;

namespace snowcoreBlog.Backend.ServiceNotifications.Extensions.Startup;

public static class AuthenticationConfigurationExtensions
{
    /// <summary>
    /// Replaces the FastEndpoints JWT plumbing with plain ASP.NET Core JWT bearer auth.
    /// Token validation semantics are kept identical (no issuer/audience validation, symmetric
    /// signing key from <c>Security:Signing:User</c>). Antiforgery services are registered for
    /// the opt-in middleware used by notification write endpoints.
    /// </summary>
    public static IServiceCollection AddApiAuthenticationConfiguration(this IServiceCollection services, string signingKey)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });

        services.AddAuthorization()
                .AddAntiforgery();

        return services;
    }

    public static WebApplication UseApiAuthenticationConfiguration(this WebApplication app)
    {
        app.UseAuthentication()
           .UseAuthorization();

        return app;
    }
}
