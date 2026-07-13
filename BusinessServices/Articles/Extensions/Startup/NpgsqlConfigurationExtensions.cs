namespace snowcoreBlog.Backend.Articles.Extensions.Startup;

public static class NpgsqlConfigurationExtensions
{
    public static WebApplicationBuilder AddNpgsqlDataSourceConfiguration(
        this WebApplicationBuilder builder,
        string connectionName)
    {
        builder.AddNpgsqlDataSource(connectionName);

        return builder;
    }
}