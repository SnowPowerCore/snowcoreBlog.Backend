using snowcoreBlog.Backend.Email.Extensions.Startup;
using snowcoreBlog.ServiceDefaults.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);

// Hosting configuration
builder.Host.UseDefaultServiceProvider(static (c, opts) =>
{
    opts.ValidateScopes = true;
    opts.ValidateOnBuild = true;
});

builder.WebHost.UseKestrelHttpsConfiguration();
builder.AddServiceDefaults();

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

app.UseHttpsRedirection();
app.MapDefaultEndpoints();

await app.RunAsync();