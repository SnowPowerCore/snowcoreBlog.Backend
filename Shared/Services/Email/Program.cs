using MassTransit;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.ServiceDefaults.Extensions;
using SendGrid.Extensions.DependencyInjection;
using FluentValidation;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.Email.Validation;
using snowcoreBlog.Backend.Email.Features.SendGrid;
using snowcoreBlog.Backend.Email.Features.Validation;

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
builder.Services.AddSendGrid(options => options.ApiKey = builder.Configuration["Integrations:SendGrid:ApiKey"]);
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<SendGenericEmailUsingSendGridConsumer>();
    busConfigurator.AddConsumer<SendTemplatedEmailUsingSendGridConsumer>();
    busConfigurator.AddConsumer<CheckEmailDomainConsumer>();
    busConfigurator.ConfigureHttpJsonOptions(static o => o.SerializerOptions.SetJsonSerializationContext());
    busConfigurator.UsingRabbitMq((context, config) =>
    {
        config.ConfigureJsonSerializerOptions(static options => options.SetJsonSerializationContext());
        config.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddSingleton<IValidator<SendGenericEmail>, GenericEmailValidator>();
builder.Services.AddSingleton<IValidator<SendTemplatedEmail>, TemplatedEmailValidator>();
builder.Services.AddSingleton<IValidator<CheckEmailDomain>, CheckEmailDomainValidator>();

var app = builder.Build();
app.UseHttpsRedirection();
app.MapDefaultEndpoints();
await app.RunAsync();