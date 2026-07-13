namespace snowcoreBlog.Backend.ServiceNotifications.Extensions.Startup;

public static class RedisConfigurationExtensions
{
    public static WebApplicationBuilder AddRedisClientConfiguration(
        this WebApplicationBuilder builder,
        string connectionName = "cache")
    {
        builder.AddRedisClient(connectionName);

        return builder;
    }
}