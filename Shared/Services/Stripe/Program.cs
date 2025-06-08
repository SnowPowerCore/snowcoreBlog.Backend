using MassTransit;
using Stripe;
using StripeMicroservice;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// MassTransit setup
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StripeMicroservice.Consumers.StripeCustomerCreatedConsumer>();
    x.AddConsumer<StripeMicroservice.Consumers.StripePaymentIntentConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

// Stripe configuration
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddSingleton<IStripeClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var apiKey = config["Stripe:ApiKey"];
    return new StripeClient(apiKey);
});

var app = builder.Build();

app.Run();
