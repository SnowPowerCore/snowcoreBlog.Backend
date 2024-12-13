using System.Security.Cryptography;
using FastEndpoints;
using FastEndpoints.OpenTelemetry.Middleware;
using FastEndpoints.Swagger;
using FluentValidation;
using Ixnas.AltchaNet;
using JasperFx.CodeGeneration;
using Marten;
using MassTransit;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Routing.Constraints;
using Oakton;
using Scalar.AspNetCore;
using snowcoreBlog.Backend.Core.Entities.Reader;
using snowcoreBlog.Backend.Core.Interfaces.Services;
using snowcoreBlog.Backend.Email.Core.Options;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Infrastructure.HttpProcessors;
using snowcoreBlog.Backend.Infrastructure.Services;
using snowcoreBlog.Backend.Infrastructure.Stores;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.Backend.ReadersManagement.Features;
using snowcoreBlog.Backend.ReadersManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.ReadersManagement.Repositories.Marten;
using snowcoreBlog.Backend.ReadersManagement.Steps.Attestation;
using snowcoreBlog.Backend.ReadersManagement.Steps.NickName;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Validation.Dto;
using snowcoreBlog.ServiceDefaults.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);
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

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.ConfigureHttpJsonOptions(static options =>
{
    options.SerializerOptions.SetJsonSerializationContext();
});

builder.Services.Configure<SendGridSenderAccountOptions>(
    builder.Configuration.GetSection("SendGrid:SenderAccount"));

builder.WebHost.UseKestrelHttpsConfiguration();
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddOpenTelemetry().ConnectBackendServices();
builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("db-snowcore-blog-entities")!);
//builder.Services.AddNpgsqlDataSource("Host=localhost;Port=54523;Username=postgres;Password=xQ6S1zf+)!kTnjFFCtt(Ks");
builder.Services.AddMarten(static opts =>
{
    opts.RegisterDocumentType<ReaderEntity>();
    opts.GeneratedCodeMode = TypeLoadMode.Static;
    opts.UseSystemTextJsonForSerialization(configure: o => o.SetJsonSerializationContext());
    opts.Policies.AllDocumentsSoftDeleted();
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
        .UseStore(() => sp.GetRequiredService<IAltchaChallengeStore>())
        .Build();
});
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<ReaderAccountTempUserCreatedConsumer>();
    busConfigurator.ConfigureHttpJsonOptions(o => o.SerializerOptions.SetJsonSerializationContext());
    busConfigurator.UsingRabbitMq((context, config) =>
    {
        config.ConfigureJsonSerializerOptions(options => options.SetJsonSerializationContext());
        config.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        config.ConfigureEndpoints(context);
    });
});
builder.Services.AddMultipleAuthentications(
   builder.Configuration["Security:Signing:User:Key"]!,
   builder.Configuration["Security:Signing:Admin:Key"]!);
builder.Services
    .AddAuthorization()
    .AddFastEndpoints(static o =>
    {
        o.SourceGeneratorDiscoveredTypes.AddRange(snowcoreBlog.Backend.ReadersManagement.DiscoveredTypes.All);
    })
    .AddAntiforgery()
    .SwaggerDocument()
    .AddCors();

builder.Services.AddSingleton<IValidator<RequestCreateReaderAccountDto>, RequestCreateReaderAccountValidation>();

builder.Services.AddScoped<IHasher, Argon2Hasher>();
builder.Services.AddScoped<IAltchaChallengeStore, AltchaChallengeStore>();
builder.Services.AddScoped<IReaderRepository, ReaderRepository>();
builder.Services.AddScoped<ValidateNickNameWasNotTakenStep>();
builder.Services.AddScoped<ValidateReaderAccountTempRecordNotExistsStep>();
builder.Services.AddScoped<ValidateReaderAccountNotExistsStep>();
builder.Services.AddScoped<ValidateReaderAccountEmailDomainStep>();
builder.Services.AddScoped<ValidateReaderAccountNickNameWasNotTakenStep>();
builder.Services.AddScoped<CreateReaderAccountTempUserStep>();
builder.Services.AddScoped<CreateUserForReaderAccountStep>();
builder.Services.AddScoped<CreateNewReaderEntityStep>();
builder.Services.AddScoped<GenerateTokenForNewReaderAccountStep>();
builder.Services.AddScoped<ReturnCreatedReaderEntityStep>();
builder.Services.AddScoped<RequestNewAttestationOptionsStep>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCookiePolicy(new()
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always
})
    .UseAuthentication()
    .UseAuthorization()
    .UseAntiforgeryFE()
    .UseFastEndpointsDiagnosticsMiddleware()
    .UseFastEndpoints(static c =>
    {
        c.Endpoints.RoutePrefix = default;
        c.Versioning.Prefix = "v";
        c.Serializer.Options.SetJsonSerializationContext();
        c.Endpoints.Configurator = ep =>
        {
            ep.PreProcessor<CookieJsonWebTokenProcessor>(Order.Before);
        };
        c.Errors.ResponseBuilder = static (failures, ctx, statusCode) =>
        {
            var failuresDict = failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(
                    keySelector: e => e.Key,
                    elementSelector: e => e.Select(m => $"{e.Key}: {m.ErrorMessage}").ToArray());

            return ErrorResponseUtilities.ApiResponseWithErrors(
                failuresDict.Values.SelectMany(x => x.Select(s => s)).ToList(), statusCode);
        };
    });

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.MapScalarApiReference(o =>
    {
        o.Servers = [];
        o.DarkMode = true;
    });
}

await app.RunOaktonCommands(args);