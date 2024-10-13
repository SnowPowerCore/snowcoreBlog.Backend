using MassTransit;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.ServiceDefaults.Extensions;
using SendGrid.Extensions.DependencyInjection;
using FluentValidation;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.Email.Validation;
using snowcoreBlog.Backend.Email.Features.SendGrid;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.SetJsonSerializationContext();
});

builder.AddServiceDefaults();
builder.Services.AddSendGrid(options => options.ApiKey = builder.Configuration["SendGrid:Key"]);
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<SendEmailUsingSendGridConsumer>();
    busConfigurator.AddConsumer<CheckEmailDomainConsumer>();
    busConfigurator.UsingRabbitMq((context, config) =>
    {
        config.ConfigureJsonSerializerOptions(options => options.SetJsonSerializationContext());
        config.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddSingleton<IValidator<SendGenericEmail>, GenericEmailValidator>();
builder.Services.AddSingleton<IValidator<CheckEmailDomain>, CheckEmailDomainValidator>();

await builder.Build().RunAsync();