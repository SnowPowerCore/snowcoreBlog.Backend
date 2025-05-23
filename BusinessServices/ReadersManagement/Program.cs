using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.OpenTelemetry.Middleware;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Ixnas.AltchaNet;
using JasperFx.CodeGeneration;
using Marten;
using MassTransit;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing.Constraints;
using MinimalStepifiedSystem.Core.Extensions;
using NSwag;
using Oakton;
using Scalar.AspNetCore;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.Backend.Core.Entities.Reader;
using snowcoreBlog.Backend.Core.Interfaces.Services;
using snowcoreBlog.Backend.Email.Core.Options;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Entities;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Infrastructure.Processors;
using snowcoreBlog.Backend.Infrastructure.Services;
using snowcoreBlog.Backend.Infrastructure.Stores;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.Backend.ReadersManagement.Features;
using snowcoreBlog.Backend.ReadersManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.ReadersManagement.Options;
using snowcoreBlog.Backend.ReadersManagement.Repositories.Marten;
using snowcoreBlog.Backend.ReadersManagement.Steps.Assertion;
using snowcoreBlog.Backend.ReadersManagement.Steps.Attestation;
using snowcoreBlog.Backend.ReadersManagement.Steps.NickName;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Confirm;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Request;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.ServiceDefaults.Extensions;

var jsonStringEnumConverter = new JsonStringEnumConverter();

var builder = WebApplication.CreateSlimBuilder(args);
builder.Host.UseDefaultServiceProvider(static (c, options) =>
{
    options.ValidateScopes = true;
    options.ValidateOnBuild = true;
});
builder.Host.ApplyOaktonExtensions();

builder.Services.Configure<MassTransitHostOptions>(static options =>
{
    options.WaitUntilStarted = true;
});

builder.Services.Configure<RouteOptions>(static options =>
{
    options.SetParameterPolicy<RegexInlineRouteConstraint>("regex");
});

builder.Services.Configure<JsonOptions>(static options =>
{
    options.SerializerOptions.SetJsonSerializationContext();
});

builder.Services.ConfigureHttpJsonOptions(static options =>
{
    options.SerializerOptions.SetJsonSerializationContext();
});

builder.Services.Configure<Argon2StringHasherOptions>(static options =>
{
    options.Strength = Argon2HashStrength.Moderate;
});

builder.Services.Configure<CookiePolicyOptions>(static options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.Configure<ForwardedHeadersOptions>(static options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.Configure<ReaderAccountTokenRequirementOptions>(
    builder.Configuration.GetSection("Security:ReaderAccountTokenRequirements"));

builder.Services.Configure<SendGridSenderAccountOptions>(
    builder.Configuration.GetSection("Integrations:Email:SenderAccount"));

builder.WebHost.UseKestrelHttpsConfiguration();
builder.AddServiceDefaults();
builder.Services.AddHttpContextAccessor();
builder.Services.AddOpenTelemetry().ConnectBackendServices();
builder.AddNpgsqlDataSource(connectionName: "db-snowcore-blog-entities");
//builder.Services.AddNpgsqlDataSource("Host=localhost;Port=54523;Username=postgres;Password=xQ6S1zf+)!kTnjFFCtt(Ks");
builder.Services.AddMarten(static options =>
{
    options.RegisterDocumentType<ReaderEntity>();
    options.RegisterDocumentType<AltchaStoredChallengeEntity>();
    options.GeneratedCodeMode = TypeLoadMode.Static;
    options.UseSystemTextJsonForSerialization(configure: static o => o.SetJsonSerializationContext());
    options.Policies.AllDocumentsSoftDeleted();
})
    .UseLightweightSessions()
    .UseNpgsqlDataSource();
builder.Services.AddSingleton(static sp =>
{
    var key = new byte[64];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(key);
    return Altcha.CreateServiceBuilder()
        .UseSha256(key)
        .SetExpiryInSeconds(30)
        .UseStore(() =>
        {
            using var scope = sp.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IAltchaCancellableChallengeStore>();
        })
        .Build();
});
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<ReaderAccountTempUserCreatedConsumer>();
    busConfigurator.ConfigureHttpJsonOptions(static o => o.SerializerOptions.SetJsonSerializationContext());
    busConfigurator.UsingRabbitMq((context, config) =>
    {
        config.ConfigureJsonSerializerOptions(static options => options.SetJsonSerializationContext());
        config.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        config.ConfigureEndpoints(context);
    });
});
builder.AddRedisClient(connectionName: "cache");

const int GlobalVersion = 1;

builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = builder.Configuration["Security:Signing:User:SigningKey"]);
builder.Services.AddAuthorization()
    .AddAntiforgery(options => options.Cookie.Expiration = TimeSpan.Zero)
    .AddFastEndpoints(static options =>
    {
        options.SourceGeneratorDiscoveredTypes.AddRange(snowcoreBlog.Backend.ReadersManagement.DiscoveredTypes.All);
    })
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
            s.OperationProcessors.Add(new AltchaHeaderProcessor());
        };
        options.SerializerSettings = s =>
        {
            s.Converters.Add(jsonStringEnumConverter);
            s.SetJsonSerializationContext();
            s.PropertyNamingPolicy = null;
            s.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        };
    });

builder.Services.AddScoped<IHasher, Argon2Hasher>();
builder.Services.AddScoped<JwtSecurityTokenHandler>();
builder.Services.AddScoped<IAltchaCancellableChallengeStore, AltchaChallengeStore>();
builder.Services.AddScoped<IReaderRepository, ReaderRepository>();
builder.Services.AddScoped<ValidateNickNameWasNotTakenStep>();
builder.Services.AddScoped<ValidateReaderAccountTempRecordNotExistsStep>();
builder.Services.AddScoped<ValidateReaderAccountExistsStep>();
builder.Services.AddScoped<snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Request.ValidateReaderAccountNotExistStep>();
builder.Services.AddScoped<snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Confirm.ValidateReaderAccountNotExistStep>();
builder.Services.AddScoped<ValidateReaderAccountEmailDomainStep>();
builder.Services.AddScoped<ValidateReaderAccountNickNameWasNotTakenStep>();
builder.Services.AddScoped<CreateReaderAccountTempUserStep>();
builder.Services.AddScoped<CreateReaderAccountUserStep>();
builder.Services.AddScoped<CreateReaderEntityForNewUserStep>();
builder.Services.AddScoped<ReturnCreatedReaderEntityStep>();
builder.Services.AddScoped<RequestNewAttestationOptionsStep>();
builder.Services.AddScoped<RequestNewAssertionOptionsStep>();
builder.Services.AddScoped<AttemptLoginByAssertionStep>();
builder.Services.AddScoped<GetTokenForReaderAccountStep>();

var app = builder.Build();

const string DefaultCulture = "en";
var supportedCultures = new[]
{
    new CultureInfo(DefaultCulture),
    new CultureInfo("tr")
};

app.UseStepifiedSystem();
app.UseHttpsRedirection()
    .UseCookiePolicy(new()
    {
        MinimumSameSitePolicy = SameSiteMode.Strict,
        HttpOnly = HttpOnlyPolicy.Always,
        Secure = CookieSecurePolicy.Always
    })
    .UseRequestLocalization(options =>
    {
        options.DefaultRequestCulture = new RequestCulture(DefaultCulture);
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;
        options.RequestCultureProviders = [
            new AcceptLanguageHeaderRequestCultureProvider()
        ];
    })
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
        c.Serializer.Options.Converters.Add(jsonStringEnumConverter);
        c.Serializer.Options.SetJsonSerializationContext();
        c.Serializer.ResponseSerializer = static (rsp, dto, contentType, _, cancellation) =>
        {
            if (dto is null)
                return Task.CompletedTask;
            rsp.ContentType = contentType;
            return rsp.WriteAsJsonAsync(
                value: dto,
                type: dto.GetType(),
                context: CoreSerializationContext.Default,
                cancellationToken: cancellation);
        };
        c.Endpoints.Configurator = static ep =>
        {
            if (ep.EndpointTags?.Contains(EndpointTagConstants.RequireCaptchaVerification) ?? false)
            {
                ep.PreProcessor<AltchaVerificationProcessor>(Order.Before);
            }
        };
        c.Errors.UseProblemDetails(static x =>
        {
            x.AllowDuplicateErrors = true;  //allows duplicate errors for the same error name
            x.IndicateErrorCode = true;     //serializes the fluentvalidation error code
            x.IndicateErrorSeverity = true; //serializes the fluentvalidation error severity
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

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseOpenApi(c =>
    {
        c.Path = "/openapi/{documentName}.json";
        c.PostProcess = (doc, req) =>
        {
            doc.Host = "https://localhost/api/readers";
            doc.Schemes = [OpenApiSchema.Https];
        };
    });
    app.MapScalarApiReference(o =>
    {
        o.DarkMode = true;
    });
}

await app.RunOaktonCommands(args);