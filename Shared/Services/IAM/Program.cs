using Fido2NetLib;
using snowcoreBlog.Backend.Core.Utilities;
using snowcoreBlog.Backend.IAM.Extensions.Startup;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.ServiceDefaults.Extensions;

[assembly: JasperFx.JasperFxAssembly]

var builder = WebApplication.CreateSlimBuilder(args);

// Hosting configuration
builder.Host.UseDefaultServiceProvider(static (c, opts) =>
{
    opts.ValidateScopes = true;
    opts.ValidateOnBuild = true;
});

builder.Services.Configure<ValidStates<HashedStringsVerificationResult>>(static options =>
{
    options.States = [HashedStringsVerificationResult.Success, HashedStringsVerificationResult.SuccessRehashNeeded];
});

builder.WebHost.UseKestrelHttpsConfiguration();
builder.AddServiceDefaults();

// OpenTelemetry
builder.Services.AddOpenTelemetry().ConnectBackendServices();

// PostgreSQL configuration
builder.AddNpgsqlDataSource("db-iam-entities", configureDataSourceBuilder: b => b.ConnectionStringBuilder.IncludeErrorDetail = true);

// Marten configuration with documents, compiled queries, and schema
builder.Services.AddMartenConfiguration();

// Identity configuration
builder.Services.AddIdentityMartenConfiguration();

// Core services
builder.Services.AddCoreServicesConfiguration();

// MassTransit configuration with consumers
builder.Services.AddMassTransitConfiguration(
    builder.Configuration.GetConnectionString("rabbitmq"));

// Fido2 configuration
builder.Services.AddFido2Configuration(builder.Configuration.GetSection(nameof(Fido2)));

// Validation services
builder.Services.AddValidationConfiguration();

var app = builder.Build();
app.UseApplicationConfiguration();
app.MapEndpoints();
await app.RunApplicationAsync();