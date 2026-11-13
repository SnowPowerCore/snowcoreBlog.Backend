using snowcoreBlog.Backend.UrlShortener.Extensions.Startup;

var builder = WebApplication.CreateSlimBuilder(args);

// Hosting configuration
builder.UseHostingConfiguration();

// Service defaults and OpenTelemetry
builder.UseServiceDefaultsConfiguration();
builder.Services.AddOpenTelemetryConfiguration();

// PostgreSQL and Marten
builder.AddNpgsqlDataSourceConfiguration("db-snowcore-blog-entities");
builder.AddRedisClientConfiguration("cache");
builder.Services.AddMartenConfiguration();

// Hosting options
builder.Services.AddHostingConfiguration();
builder.Services.AddUrlShortenerOptionsConfiguration(builder.Configuration);

// Core services
builder.Services.AddCoreServicesConfiguration();
builder.Services.AddUrlMappingRepositoriesConfiguration();

// FastEndpoints
builder.Services.AddFastEndpointsConfiguration();

var app = builder.Build();

// Middleware pipeline
app.UseApplicationMiddleware();

// Endpoints
app.MapEndpoints();

await app.RunApplicationAsync();