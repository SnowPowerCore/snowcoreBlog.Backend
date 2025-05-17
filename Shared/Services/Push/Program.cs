using MassTransit;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.ServiceDefaults.Extensions;
using FluentValidation;
using snowcoreBlog.Backend.Push.Validation;
using snowcoreBlog.Backend.Push.Core.Contracts;
using snowcoreBlog.Backend.Push.Features.Ntfy;

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
    options.SerializerOptions.SetJsonSerializationContext();
});

builder.WebHost.UseKestrelHttpsConfiguration();
builder.AddServiceDefaults();
builder.Services.AddNtfyCator(options =>
{
    options.Uri = "http://localhost:4010";
});
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<SendPushUsingNtfyConsumer>();
    busConfigurator.ConfigureHttpJsonOptions(static o => o.SerializerOptions.SetJsonSerializationContext());
    busConfigurator.UsingRabbitMq((context, config) =>
    {
        config.ConfigureJsonSerializerOptions(options => options.SetJsonSerializationContext());
        config.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddSingleton<IValidator<SendGenericPush>, GenericPushValidator>();

await builder.Build().RunAsync();