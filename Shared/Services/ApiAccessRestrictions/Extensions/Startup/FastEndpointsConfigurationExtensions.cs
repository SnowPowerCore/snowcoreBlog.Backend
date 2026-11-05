using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.PublicApi.Extensions;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Extensions.Startup;

public static class FastEndpointsConfigurationExtensions
{
    const int GlobalVersion = 1;
    private static readonly JsonStringEnumConverter JsonStringEnumConverter = new();

    public static IServiceCollection AddFastEndpointsConfiguration(this IServiceCollection services, string signingKey)
    {
        services.AddAuthenticationJwtBearer(options => options.SigningKey = signingKey);
        services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
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
        });

        services.AddAuthorization()
                .AddFastEndpoints()
                .SwaggerDocument(options =>
                {
                    options.AutoTagPathSegmentIndex = 0;
                    options.ShortSchemaNames = true;
                    options.MaxEndpointVersion = GlobalVersion;
                    options.SerializerSettings = s =>
                    {
                        s.Converters.Add(JsonStringEnumConverter);
                        s.SetJsonSerializationContext();
                        s.PropertyNamingPolicy = null;
                    };
                });

        return services;
    }

    public static WebApplication UseFastEndpointsConfiguration(this WebApplication app)
    {
        app.UseAuthentication()
            .UseAuthorization()
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
                c.Serializer.Options.Converters.Add(JsonStringEnumConverter);
                c.Serializer.Options.SetJsonSerializationContext();
            });

        return app;
    }
}