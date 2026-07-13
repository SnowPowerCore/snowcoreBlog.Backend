namespace snowcoreBlog.Backend.IAM.Extensions.Startup;

public static class Fido2ConfigurationExtensions
{
    public static IServiceCollection AddFido2Configuration(this IServiceCollection services, IConfigurationSection configuration)
    {
        services.AddFido2(configuration);

        return services;
    }
}