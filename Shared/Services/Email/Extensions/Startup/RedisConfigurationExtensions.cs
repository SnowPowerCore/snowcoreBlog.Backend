using StackExchange.Redis;

namespace snowcoreBlog.Backend.Email.Extensions.Startup;

public static class RedisConfigurationExtensions
{
    public static WebApplicationBuilder AddRedisClientConfiguration(
        this WebApplicationBuilder builder,
        string connectionName = "cache")
    {
        builder.AddRedisClient(connectionName);

        return builder;
    }

    public static IServiceCollection AddRedisClientConfiguration(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(connectionString));

        return services;
    }
}