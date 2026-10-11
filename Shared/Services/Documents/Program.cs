using snowcoreBlog.Backend.Documents.Extensions.Startup;

var builder = WebApplication.CreateSlimBuilder(args);

// Hosting configuration
builder.UseServiceDefaultsConfiguration();

// OpenTelemetry
builder.Services.AddTelemetryConfiguration();

// PostgreSQL and Marten
builder.AddNpgsqlDataSourceConfiguration("db-documents-entities");
builder.Services.AddMartenConfiguration();

// MassTransit configuration
builder.Services.AddMassTransitConfiguration(
    builder.Configuration.GetConnectionString("rabbitmq"));

// Hosting options
builder.Services.AddHostingConfiguration();

// FastEndpoints and Authentication
builder.Services.AddFastEndpointsConfiguration(
    builder.Configuration["Security:Signing:User:SigningKey"] ?? throw new InvalidOperationException("Signing key not configured"));

// Documents options (upload limits + content storage root)
builder.Services.AddDocumentsOptionsConfiguration(builder.Configuration, "Documents");

// Core services (repositories, content store, processing steps)
builder.Services.AddCoreServicesConfiguration();

var app = builder.Build();

// Middleware pipeline
app.UseApplicationMiddleware();

// Endpoints
app.MapEndpoints();

await app.RunApplicationAsync();
