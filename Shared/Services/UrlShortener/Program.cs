using snowcoreBlog.Backend.UrlShortener.Extensions.Startup;

var builder = WebApplication.CreateSlimBuilder(args);

// Hosting configuration
builder.UseServiceDefaultsConfiguration();

// OpenTelemetry
builder.Services.AddTelemetryConfiguration();

// PostgreSQL and Marten
builder.AddNpgsqlDataSourceConfiguration("db-snowcore-blog-entities");
builder.AddRedisClientConfiguration("cache");

// Hosting options
builder.Services.AddHostingConfiguration();

// FastEndpoints
builder.Services.AddFastEndpointsConfiguration();

var app = builder.Build();

// Middleware pipeline
app.UseApplicationMiddleware();

// Endpoints
app.MapEndpoints();

await app.RunApplicationAsync();