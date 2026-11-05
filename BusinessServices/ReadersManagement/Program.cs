using System.Text.Json.Serialization.Metadata;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Infrastructure.RequestPersistence.ReplayAuthentication;
using snowcoreBlog.Backend.ReadersManagement;
using snowcoreBlog.Backend.ReadersManagement.Extensions.Startup;
using snowcoreBlog.Backend.ReadersManagement.RequestPersistence;
[assembly: JasperFx.JasperFxAssembly]

var builder = WebApplication.CreateSlimBuilder(args);

// Hosting configuration
builder.UseHostingConfiguration();
builder.UseServiceDefaultsConfiguration();

// OpenTelemetry
builder.Services.AddOpenTelemetryConfiguration();

// PostgreSQL and Marten
builder.AddNpgsqlDataSourceConfiguration("db-snowcore-blog-entities");

// Marten and Altcha configuration
builder.Services.AddMartenConfiguration();

// Durable request persistence: claims, step checkpoints, recovery worker, replay machinery
builder.Services.AddPersistRequestProcessing(options =>
    builder.Configuration.GetSection("PersistRequest").Bind(options));
builder.Services.AddSingleton<IJsonTypeInfoResolver>(static _ => ReadersCheckpointJsonContext.Default);
builder.Services.AddSingleton<IReplayAuthenticationProvider, ReadersReplayAuthenticationProvider>();

// Redis cache
builder.AddRedisClientConfiguration("cache");

// TickerQ: scheduler for the durable request persistence recovery poll (RecoveryPollInterval
// cron seeded at startup) with the TickerQ dashboard backed by the shared Redis instance.
builder.Services.AddTickerQConfiguration(builder.Configuration);

// MassTransit configuration
builder.Services.AddMassTransitConfiguration(builder.Configuration.GetConnectionString("rabbitmq"));

// Project and Security options
builder.Services.AddProjectOptionsConfiguration(builder.Configuration, "Project");
builder.Services.AddTokenRequirementsConfiguration(builder.Configuration, "Security:ReaderAccountTokenRequirements");
builder.Services.AddEmailSenderConfiguration(builder.Configuration, "Integrations:Email:SenderAccount");

// Hosting options
builder.Services.AddHostingConfiguration();

// FastEndpoints and Authentication
builder.Services.AddFastEndpointsConfiguration(
    builder.Configuration["Security:Signing:User:SigningKey"] ?? throw new InvalidOperationException("Signing key not configured"));

// External identity providers (Microsoft / Google / Apple) — schemes are only
// registered when their credentials are present in Integrations:ExternalAuth.
builder.Services.AddExternalAuthenticationConfiguration(builder.Configuration);

// Core services
builder.Services.AddCoreServicesConfiguration();

// ActualLab.Fusion: real-time nickname availability compute service + RPC WebSocket server
builder.Services.AddFusionConfiguration();

var app = builder.Build();

// Middleware pipeline
app.UseApplicationMiddleware();

// Fusion: enable WebSockets + map the RPC WebSocket endpoint (/fusion/ws) before endpoints
app.UseFusionConfiguration();

// Endpoints
app.MapEndpoints();

// TickerQ: eager checkpoint-coordinator construction, recovery cron seeding, scheduler start
await app.UseTickerQConfigurationAsync();

await app.RunApplicationAsync();