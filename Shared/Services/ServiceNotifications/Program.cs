using snowcoreBlog.Backend.ServiceNotifications.Extensions.Startup;

var builder = WebApplication.CreateSlimBuilder(args);
// Hosting configuration
builder.UseServiceDefaultsConfiguration();

// OpenTelemetry
builder.Services.AddTelemetryConfiguration();

// PostgreSQL and Marten
builder.AddNpgsqlDataSourceConfiguration("db-snowcore-blog-entities");
builder.Services.AddMartenConfiguration();

// Redis cache
builder.AddRedisClientConfiguration("cache");

// MassTransit configuration
builder.Services.AddMassTransitConfiguration(
    builder.Configuration.GetConnectionString("rabbitmq"));

// Hosting options
builder.Services.AddHostingConfiguration();

// FastEndpoints and Authentication
builder.Services.AddFastEndpointsConfiguration(
    builder.Configuration["Security:Signing:User:SigningKey"] ?? throw new InvalidOperationException("Signing key not configured"));

// Core services
builder.Services.AddCoreServicesConfiguration();

var app = builder.Build();

// Middleware pipeline
app.UseApplicationMiddleware();

// Endpoints
app.MapEndpoints();

await app.RunApplicationAsync();