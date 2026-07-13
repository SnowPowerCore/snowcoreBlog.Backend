using snowcoreBlog.Backend.Push.Extensions.Startup;
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

// Ntfy configuration
builder.Services.AddNtfyConfiguration();

// MassTransit configuration
builder.Services.AddMassTransitConfiguration(
    builder.Configuration.GetConnectionString("rabbitmq"));

// Validation
builder.Services.AddValidationConfiguration();

await builder.Build().RunAsync();