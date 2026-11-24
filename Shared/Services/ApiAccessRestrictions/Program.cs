using snowcoreBlog.Backend.ApiAccessRestrictions.Extensions.Startup;

var builder = WebApplication.CreateSlimBuilder(args);

// Hosting configuration
builder.UseServiceDefaultsConfiguration();

// PostgreSQL and Marten
builder.AddNpgsqlDataSourceConfiguration("db-ip-restrictions-entities");
builder.Services.AddMartenConfiguration();

// Redis cache
builder.AddRedisClientConfiguration("cache");

// MassTransit configuration
builder.Services.AddMassTransitConfiguration(
    builder.Configuration.GetConnectionString("rabbitmq"));

// Hosting options
builder.Services.AddHostingConfiguration();

// OpenAPI document generation (development only mapping)
builder.Services.AddOpenTelemetryConfiguration();

// FastEndpoints + JWT auth (admin token cookie lifted into the bearer context)
builder.Services.AddFastEndpointsConfiguration();
builder.Services.AddApiAuthenticationConfiguration(
    builder.Configuration["Security:Signing:Admin:SigningKey"]
        ?? throw new InvalidOperationException("Admin signing key not configured"));

// Core services
builder.Services.AddCoreServicesConfiguration();

var app = builder.Build();

// Middleware pipeline
app.UseApplicationMiddleware();

// Endpoints
app.MapEndpoints();

await app.RunApplicationAsync();