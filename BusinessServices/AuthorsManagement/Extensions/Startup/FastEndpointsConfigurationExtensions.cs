using FastEndpoints;
using FastEndpoints.OpenTelemetry.Middleware;
using FastEndpoints.Security;
using Microsoft.IdentityModel.Tokens;
using snowcoreBlog.Backend.Gateway.Middleware.Middleware;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.PublicApi.Extensions;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace snowcoreBlog.Backend.AuthorsManagement.Extensions.Startup;

public static class FastEndpointsConfigurationExtensions
{
    const int GlobalVersion = 1;

    public static IServiceCollection AddFastEndpointsConfiguration(
        this IServiceCollection services,
        string signingKey)
    {
        // Registers the JwtBearer scheme as the DEFAULT authentication scheme (and thus
        // the default challenge scheme) — without it, endpoints with Claims(...)/[Authorize]
        // throw "No authenticationScheme was specified, and there was no DefaultChallengeScheme found"
        // when an unauthenticated request challenges.
        services.AddAuthenticationJwtBearer(s => s.SigningKey = signingKey);

        // Mirror the signing configuration used by JwtTokenCreator (gateway) so endpoints
        // can both verify AND read claims from a token issued by the gateway.
        // JwtTokenCreator signs with: new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signingKey))
        services.AddSingleton(new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signingKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        });

        services.AddAuthorization()
                .AddAntiforgery()
                .AddFastEndpoints();

        return services;
    }

    public static WebApplication UseFastEndpointsConfiguration(this WebApplication app)
    {
        app.UseMiddleware<UserCookieJsonWebTokenMiddleware>()
            .UseAuthentication()
            .UseAuthorization()
            .UseAntiforgeryFE(additionalContentTypes: [MediaTypeNames.Application.Json])
            .UseFastEndpointsDiagnosticsMiddleware()
            .UseFastEndpoints(c =>
            {
                c.Endpoints.NameGenerator = static ctx =>
                {
                    var currentName = ctx.EndpointType.Name;
                    return currentName.TrimEnd("Endpoint");
                };
                c.Endpoints.ShortNames = true;
                c.Endpoints.RoutePrefix = default;
                c.Versioning.Prefix = "v";
                c.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                c.Serializer.Options.SetJsonSerializationContext();
            });

        return app;
    }
}