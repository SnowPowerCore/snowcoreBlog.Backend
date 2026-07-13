namespace snowcoreBlog.Backend.Push.Extensions.Startup;

public static class NtfyConfigurationExtensions
{
    public static IServiceCollection AddNtfyConfiguration(this IServiceCollection services, string uri = "http://localhost:4010")
    {
        services.AddNtfyCator(options =>
        {
            options.Uri = uri;
        });

        return services;
    }
}