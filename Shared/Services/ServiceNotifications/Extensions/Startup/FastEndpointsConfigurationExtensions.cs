using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.OpenTelemetry.Middleware;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.IdentityModel.Tokens;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Infrastructure.Processors;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.PublicApi.Extensions;

namespace snowcoreBlog.Backend.ServiceNotifications.Extensions.Startup;

public static class FastEndpointsConfigurationExtensions
{
    const int GlobalVersion = 1;
    private static readonly JsonStringEnumConverter JsonStringEnumConverter = new();

    public static IServiceCollection AddFastEndpointsConfiguration(
        this IServiceCollection services,
        string signingKey)
    {
        // Registers the JwtBearer scheme as the DEFAULT authentication scheme (and thus
        // the default challenge scheme) — without it, endpoints with Claims(...) throw
        // "No authenticationScheme was specified, and there was no DefaultChallengeScheme found"
        // when an unauthenticated request challenges. Mirrors Articles.
        services.AddAuthenticationJwtBearer(s => s.SigningKey = signingKey);

        // Mirror the signing configuration used by JwtTokenCreator (gateway) so endpoints
        // can both verify AND read claims from a token issued by the gateway.
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
                .AddFastEndpoints()
                .SwaggerDocument(options =>
                {
                    options.AutoTagPathSegmentIndex = 0;
                    options.ShortSchemaNames = true;
                    options.MaxEndpointVersion = GlobalVersion;
                    options.DocumentSettings = static s =>
                    {
                        s.DocumentName = $"v{GlobalVersion}";
                        s.Version = $"v{GlobalVersion}";
                        s.SchemaSettings.IgnoreObsoleteProperties = true;
                        s.OperationProcessors.Add(new AntiforgeryHeaderProcessor());
                    };
                    options.SerializerSettings = s =>
                    {
                        s.Converters.Add(JsonStringEnumConverter);
                        s.SetJsonSerializationContext(ServiceNotificationsSerializationContext.Default);
                        s.PropertyNamingPolicy = null;
                        s.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    };
                })
                .AddResponseCaching();

        return services;
    }

    public static WebApplication UseFastEndpointsConfiguration(this WebApplication app)
    {
        app.UseResponseCaching()
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
                c.Serializer.Options.Converters.Add(JsonStringEnumConverter);
                c.Serializer.Options.SetJsonSerializationContext(ServiceNotificationsSerializationContext.Default);
                c.Serializer.ResponseSerializer = static (rsp, dto, contentType, _, cancellation) =>
                {
                    if (dto is default(object))
                        return Task.CompletedTask;
                    rsp.ContentType = contentType;
                    return rsp.WriteAsJsonAsync(
                        value: dto,
                        type: dto.GetType(),
                        context: ServiceNotificationsSerializationContext.Default,
                        cancellationToken: cancellation);
                };
                c.Errors.UseProblemDetails(static x =>
                {
                    x.AllowDuplicateErrors = true;
                    x.IndicateErrorCode = true;
                    x.IndicateErrorSeverity = true;
                    x.TypeValue = "https://www.rfc-editor.org/rfc/rfc7231#section-6.5.1";
                    x.TitleValue = "One or more validation errors occurred.";
                    x.TitleTransformer = static pd => pd.Status switch
                    {
                        400 => "Validation Error",
                        404 => "Not Found",
                        _ => "One or more errors occurred!"
                    };
                });
                c.Errors.ResponseBuilder = static (failures, ctx, statusCode) =>
                {
                    var failuresDict = failures
                        .GroupBy(static f => f.PropertyName)
                        .ToDictionary(
                            keySelector: static e => e.Key,
                            elementSelector: static e => e.Select(m => $"{e.Key}: {m.ErrorMessage}").ToArray());

                    return ErrorResponseUtilities.ApiResponseWithErrors(
                        failuresDict.Values.SelectMany(static x => x.Select(static s => s)).ToList(), statusCode);
                };
            });

        return app;
    }
}
