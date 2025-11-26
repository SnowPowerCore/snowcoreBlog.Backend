using System.Text.Json.Serialization;
using FluentValidation;
using MassTransit;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Push.Core.Contracts;
using snowcoreBlog.Backend.Push.Features.Ntfy;
using snowcoreBlog.Backend.Push.Validation;
using snowcoreBlog.ServiceDefaults.Extensions;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Host.UseDefaultServiceProvider(static (c, opts) =>
{
    opts.ValidateScopes = true;
    opts.ValidateOnBuild = true;
});

builder.Services.Configure<MassTransitHostOptions>(static options =>
{
    options.WaitUntilStarted = true;
});

builder.Services.ConfigureHttpJsonOptions(static options =>
{
    options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
    options.SerializerOptions.SetJsonSerializationContext();
});

builder.WebHost.UseKestrelHttpsConfiguration();
builder.AddServiceDefaults();
builder.Services.AddNtfyCator(static options =>
{
    options.Uri = "http://localhost:4010";
});
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<SendPushUsingNtfyConsumer>();
    busConfigurator.ConfigureHttpJsonOptions(static o =>
    {
        o.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        o.SerializerOptions.SetJsonSerializationContext();
    });
    busConfigurator.UsingRabbitMq((context, config) =>
    {
        config.ConfigureJsonSerializerOptions(static options => options.SetJsonSerializationContext());
        config.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddSingleton<IValidator<SendGenericPush>, GenericPushValidator>();

await builder.Build().RunAsync();