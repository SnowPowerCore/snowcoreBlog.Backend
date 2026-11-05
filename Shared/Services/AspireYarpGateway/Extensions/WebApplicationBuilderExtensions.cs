using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using snowcoreBlog.Backend.AspireYarpGateway.Constants;
using snowcoreBlog.Backend.Core.Constants;

namespace snowcoreBlog.Backend.AspireYarpGateway.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static AuthenticationBuilder AddMultipleAuthentications(this IServiceCollection services, params string[] keys) =>
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = SecurityConstants.UserSchemeName;
                options.DefaultSignInScheme = SecurityConstants.UserSchemeName;
                options.DefaultChallengeScheme = SecurityConstants.UserSchemeName;
            })
            .AddJwtBearer(SecurityConstants.UserSchemeName, options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = SecurityConstants.UserIssuer,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(keys.First())
                    )
                };
            })
            .AddJwtBearer(SecurityConstants.AdminSchemeName, options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var adminToken = context.Request.Cookies[AuthCookieConstants.AdminAccessTokenCookieName];
                        if (!string.IsNullOrWhiteSpace(adminToken))
                        {
                            context.Token = adminToken;
                        }

                        return Task.CompletedTask;
                    }
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = SecurityConstants.AdminIssuer,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(keys.Last())
                    )
                };
            });
}