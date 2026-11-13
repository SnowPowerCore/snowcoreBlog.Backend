using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using snowcoreBlog.Backend.Core.Constants;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Extensions.Startup;

public static class AuthenticationConfigurationExtensions
{
    /// <summary>
    /// Replaces the FastEndpoints JWT plumbing with plain ASP.NET Core JWT bearer auth.
    /// Token validation semantics are kept identical (no issuer/audience validation, symmetric
    /// signing key) and the admin access-token cookie is lifted into the bearer token context.
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

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = static context =>
                        {
                            var adminToken = context.Request.Cookies[AuthCookieConstants.AdminAccessTokenCookieName];
                            if (!string.IsNullOrWhiteSpace(adminToken))
                            {
                                context.Token = adminToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

        services.AddAuthorization();

        return services;
    }

    public static WebApplication UseApiAuthenticationConfiguration(this WebApplication app)
    {
        app.UseAuthentication()
           .UseAuthorization();

        return app;
    }
}
