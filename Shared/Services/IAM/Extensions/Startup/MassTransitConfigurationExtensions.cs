using System.Text.Json.Serialization;
using MassTransit;
using snowcoreBlog.Backend.IAM.Features.Admin;
using snowcoreBlog.Backend.IAM.Features.User;
using snowcoreBlog.Backend.Infrastructure.Extensions;

namespace snowcoreBlog.Backend.IAM.Extensions.Startup;

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
            busConfigurator.AddConsumer<CheckAndPerformAssertionConsumer>();
            busConfigurator.AddConsumer<ValidateAndCreateUserConsumer>();
            busConfigurator.AddConsumer<CreateTempUserConsumer>();
            busConfigurator.AddConsumer<ValidateUserExistsConsumer>();
            busConfigurator.AddConsumer<ValidateTempUserExistsConsumer>();
            busConfigurator.AddConsumer<ValidateUserNickNameWasTakenConsumer>();
            busConfigurator.AddConsumer<ValidateAndCreateAttestationConsumer>();
            busConfigurator.AddConsumer<ValidateAndCreateAssertionConsumer>();
            busConfigurator.AddConsumer<ValidateAdminExistsConsumer>();
            busConfigurator.AddConsumer<InviteAndCreateAdminConsumer>();
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