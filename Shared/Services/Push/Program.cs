using snowcoreBlog.Backend.Push.Extensions.Startup;

var builder = WebApplication.CreateSlimBuilder(args);

// Hosting configuration
builder.UseHostingConfiguration();

// Service defaults and OpenTelemetry
builder.UseServiceDefaultsConfiguration();

// Ntfy configuration
builder.Services.AddNtfyConfiguration();

// MassTransit configuration
builder.Services.AddMassTransitConfiguration(
    builder.Configuration.GetConnectionString("rabbitmq"));

// Validation
builder.Services.AddValidationConfiguration();

var app = builder.Build();

app.UseApplicationMiddleware();
app.MapEndpoints();
await app.RunApplicationAsync();