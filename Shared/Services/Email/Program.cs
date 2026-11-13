using snowcoreBlog.Backend.Email.Extensions.Startup;

var builder = WebApplication.CreateSlimBuilder(args);

// Hosting configuration
builder.UseHostingConfiguration();

// Service defaults and OpenTelemetry
builder.UseServiceDefaultsConfiguration();

// Redis cache
builder.AddRedisClientConfiguration("cache");

// Email services (SendGrid, AWS SES)
builder.Services.AddEmailServiceConfiguration(builder.Configuration);

// MassTransit configuration
builder.Services.AddMassTransitConfiguration(
    builder.Configuration.GetConnectionString("rabbitmq"));

// Validation services
builder.Services.AddValidationConfiguration();

// External API configuration (Apizr for disposable email detection)
builder.Services.AddExternalApiConfiguration();

var app = builder.Build();

app.UseApplicationMiddleware();
app.MapEndpoints();
await app.RunApplicationAsync();