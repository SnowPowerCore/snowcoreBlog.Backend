using snowcoreBlog.Backend.Articles.Extensions.Startup;

[assembly: JasperFx.JasperFxAssembly]

var builder = WebApplication.CreateSlimBuilder(args);

// Hosting configuration
builder.UseHostingConfiguration();

// PostgreSQL and Marten
builder.AddNpgsqlDataSourceConfiguration(connectionName: "db-snowcore-blog-article-entities");
builder.Services.AddMartenConfiguration();

// Redis cache
builder.AddRedisClientConfiguration(connectionName: "cache");

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