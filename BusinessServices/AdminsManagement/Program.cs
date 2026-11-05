using snowcoreBlog.Backend.AdminsManagement.Extensions.Startup;

var builder = WebApplication.CreateSlimBuilder(args);
// Hosting configuration
builder.UseServiceDefaultsConfiguration();

// OpenTelemetry
builder.Services.AddTelemetryConfiguration();

// PostgreSQL and Marten
builder.AddNpgsqlDataSourceConfiguration("db-admins-entities");
builder.Services.AddMartenConfiguration();

// Redis cache
builder.AddRedisClientConfiguration("cache");

// MassTransit configuration
builder.Services.AddMassTransitConfiguration(
    builder.Configuration.GetConnectionString("rabbitmq"));

// Hosting options
builder.Services.AddHostingConfiguration();

// FastEndpoints and Authentication (admin-scheme JWTs: issuer "admin", admin signing key)
builder.Services.AddFastEndpointsConfiguration(
    builder.Configuration["Security:Signing:Admin:SigningKey"] ?? throw new InvalidOperationException("Admin signing key not configured"));

// Core services
builder.Services.AddCoreServicesConfiguration(builder.Configuration);

var app = builder.Build();

// Middleware pipeline
app.UseApplicationMiddleware();

// Endpoints
app.MapEndpoints();

await app.RunApplicationAsync();