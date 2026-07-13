using snowcoreBlog.Backend.ReadersManagement.Extensions.Startup;

[assembly: JasperFx.JasperFxAssembly]

var builder = WebApplication.CreateSlimBuilder(args);

// Hosting configuration
builder.UseHostingConfiguration();
builder.UseServiceDefaultsConfiguration();

// OpenTelemetry
builder.Services.AddOpenTelemetryConfiguration();

// PostgreSQL and Marten
builder.AddNpgsqlDataSourceConfiguration("db-snowcore-blog-entities");

// Marten and Altcha configuration
builder.Services.AddMartenConfiguration();

// Redis cache
builder.AddRedisClientConfiguration("cache");

// MassTransit configuration
builder.Services.AddMassTransitConfiguration(builder.Configuration.GetConnectionString("rabbitmq"));

// Project and Security options
builder.Services.AddProjectOptionsConfiguration(builder.Configuration, "Project");
builder.Services.AddTokenRequirementsConfiguration(builder.Configuration, "Security:ReaderAccountTokenRequirements");
builder.Services.AddEmailSenderConfiguration(builder.Configuration, "Integrations:Email:SenderAccount");

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