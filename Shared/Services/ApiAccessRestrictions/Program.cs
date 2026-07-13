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

// OpenTelemetry
builder.Services.AddTelemetryConfiguration();

// FastEndpoints
builder.Services.AddFastEndpointsConfiguration();

var app = builder.Build();

// Middleware pipeline
app.UseApplicationMiddleware();

// Endpoints
app.MapEndpoints();

await app.RunApplicationAsync();