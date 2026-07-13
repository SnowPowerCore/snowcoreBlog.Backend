using System.Text.Json.Serialization;
using MassTransit;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Push.Features.Ntfy;

namespace snowcoreBlog.Backend.Push.Extensions.Startup;

public static class MassTransitConfigurationExtensions
{
    public static IServiceCollection AddMassTransitConfiguration(
        this IServiceCollection services,
        string rabbitMqConnectionString)
    {
        services.Configure<MassTransitHostOptions>(static options =>
        {
            options.WaitUntilStarted = true;
        });

        services.AddMassTransit(busConfigurator =>
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
                config.Host(rabbitMqConnectionString);
                config.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}