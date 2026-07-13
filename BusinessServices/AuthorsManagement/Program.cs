using snowcoreBlog.Backend.AuthorsManagement.Extensions.Startup;

var builder = WebApplication.CreateSlimBuilder(args);

// Hosting configuration
builder.UseHostingConfiguration();

// PostgreSQL and Marten
builder.AddNpgsqlDataSource("db-snowcore-blog-entities");
builder.Services.AddMartenConfiguration();

// Redis cache
builder.AddRedisClientConfiguration("cache");

builder.Services.AddCompiledQueriesConfiguration();

// MassTransit configuration
builder.Services.AddMassTransitConfiguration(
    builder.Configuration.GetConnectionString("rabbitmq"));

// Hosting options
builder.Services.AddHostingConfiguration();

// FastEndpoints
builder.Services.AddFastEndpointsConfiguration();

// Core services
builder.Services.AddCoreServicesConfiguration();

var app = builder.Build();

// Middleware pipeline
app.UseApplicationMiddleware();

// Endpoints
app.MapEndpoints();

await app.RunApplicationAsync();