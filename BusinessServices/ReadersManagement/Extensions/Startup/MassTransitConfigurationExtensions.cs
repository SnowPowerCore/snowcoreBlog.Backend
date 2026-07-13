using System.Text.Json.Serialization;
using MassTransit;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.ReadersManagement.Features;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions.Startup;

public static class MassTransitConfigurationExtensions
{
    public static IServiceCollection AddMassTransitConfiguration(
        this IServiceCollection services,
        string rabbitMqConnectionString,
        Action<IRegistrationConfigurator>? configurator = null)
    {
        services.Configure<MassTransitHostOptions>(static options =>
        {
            options.WaitUntilStarted = true;
        });

        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddConsumer<ReaderAccountTempUserCreatedConsumer>();
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
            configurator?.Invoke(busConfigurator);
        });

        return services;
    }
}