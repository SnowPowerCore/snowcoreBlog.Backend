using snowcoreBlog.Backend.Stripe.Extensions.Startup;

var builder = WebApplication.CreateSlimBuilder(args);

// Hosting configuration
builder.UseServiceDefaultsConfiguration();

// OpenTelemetry
builder.Services.AddTelemetryConfiguration();

// PostgreSQL and Marten
builder.AddNpgsqlDataSourceConfiguration("db-stripe-entities");
builder.Services.AddMartenConfiguration();

// MassTransit configuration
builder.Services.AddMassTransitConfiguration(
    builder.Configuration.GetConnectionString("rabbitmq"));

// Hosting options
builder.Services.AddHostingConfiguration();

// Stripe SDK client + customer/webhook domain services
builder.Services.AddStripeClientConfiguration(builder.Configuration, "Stripe");

// FastEndpoints and Authentication
builder.Services.AddFastEndpointsConfiguration(
    builder.Configuration["Security:Signing:User:SigningKey"] ?? throw new InvalidOperationException("Signing key not configured"));

// Stripe options (keys, currency allowlist, amount bounds, redirects)
builder.Services.AddStripeOptionsConfiguration(builder.Configuration, "Stripe");

// Core services (repositories, checkout pipeline steps)
builder.Services.AddCoreServicesConfiguration();

var app = builder.Build();

// Middleware pipeline
app.UseApplicationMiddleware();

// Endpoints
app.MapEndpoints();

await app.RunApplicationAsync();
