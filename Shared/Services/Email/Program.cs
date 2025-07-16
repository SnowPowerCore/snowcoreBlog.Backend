using MassTransit;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.ServiceDefaults.Extensions;
using SendGrid.Extensions.DependencyInjection;
using FluentValidation;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.Email.Validation;
using snowcoreBlog.Backend.Email.Features.SendGrid;
using snowcoreBlog.Backend.Email.Features.Validation;
using Amazon.SimpleEmailV2;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon;
using snowcoreBlog.Backend.Email.Features.AmazonSimpleEmail;
using Apizr;
using snowcoreBlog.Backend.Email.Api;
using Apizr.Extending.Configuring.Common;
using Microsoft.Extensions.Http.Resilience;

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
builder.Services.AddAWSService<IAmazonSimpleEmailServiceV2>(new AWSOptions
{
    Credentials = new BasicAWSCredentials(string.Empty, string.Empty),
    Region = RegionEndpoint.EUNorth1
});
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.AddConsumer<SendGenericEmailUsingAmazonSimpleEmailConsumer>();
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
builder.AddRedisClient(connectionName: "cache");

builder.Services.AddSingleton<IValidator<SendGenericEmail>, GenericEmailValidator>();
builder.Services.AddSingleton<IValidator<SendTemplatedEmail>, TemplatedEmailValidator>();
builder.Services.AddSingleton<IValidator<CheckEmailDomain>, CheckEmailDomainValidator>();

Action<IApizrExtendedCommonOptionsBuilder> optionsBuilder = options =>
{
    options.ConfigureHttpClientBuilder(builder => builder
        .AddStandardResilienceHandler(config =>
        {
            config.Retry = new HttpRetryStrategyOptions
            {
                UseJitter = true,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(0.5)
            };
        }))
        .WithPriority();
};

builder.Services.AddApizr(
    registry => registry
        .AddManagerFor<IEmailDisposableApi>(opts => opts.WithBaseAddress("https://disposable.github.io"))
        .AddManagerFor<IStaticEmailDisposableApi>(opts => opts.WithBaseAddress("https://rawcdn.githack.com")),
    optionsBuilder);

var app = builder.Build();
app.UseHttpsRedirection();
app.MapDefaultEndpoints();
await app.RunAsync();