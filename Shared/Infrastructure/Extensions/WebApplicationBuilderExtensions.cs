using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using snowcoreBlog.Backend.IAM.Core.Constants;

namespace snowcoreBlog.Backend.Infrastructure.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static AuthenticationBuilder AddMultipleAuthentications(this IServiceCollection services, params string[] keys) =>
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthConstants.UserAuthSchemeName;
                options.DefaultSignInScheme = AuthConstants.UserAuthSchemeName;
                options.DefaultChallengeScheme = AuthConstants.UserAuthSchemeName;
            })
            .AddJwtBearer(AuthConstants.UserAuthSchemeName, options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = "user",
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(keys.First())
                    )
                };
            })
            .AddJwtBearer(AuthConstants.AdminAuthSchemeName, options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "admin",
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(keys.Last())
                    )
                };
            });
}