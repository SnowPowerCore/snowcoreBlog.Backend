using snowcoreBlog.Backend.AuthorsManagement.Extensions.Startup;

var builder = WebApplication.CreateSlimBuilder(args);

// Hosting configuration
builder.UseHostingConfiguration();

// PostgreSQL and Marten
builder.AddNpgsqlDataSource("db-snowcore-blog-entities");
builder.Services.AddMartenConfiguration();

// Redis cache
builder.AddRedisClientConfiguration("cache");

// MassTransit configuration
builder.Services.AddMassTransitConfiguration(
    builder.Configuration.GetConnectionString("rabbitmq"));

// Hosting options
builder.Services.AddHostingConfiguration();

// FastEndpoints
builder.Services.AddFastEndpointsConfiguration(
    builder.Configuration["Security:Signing:User:SigningKey"]
        ?? throw new InvalidOperationException("Signing key not configured"));

// Core services
builder.Services.AddCoreServicesConfiguration();

// ActualLab.Fusion: real-time author display name availability compute service + RPC WebSocket server
builder.Services.AddFusionConfiguration();

var app = builder.Build();

// Middleware pipeline
app.UseApplicationMiddleware();

// Fusion: enable WebSockets + map the RPC WebSocket endpoint (/fusion/ws) before endpoints
app.UseFusionConfiguration();

// Endpoints
app.MapEndpoints();

await app.RunApplicationAsync();