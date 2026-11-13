namespace snowcoreBlog.Backend.Email.Extensions.Startup;

public static class HostingConfigurationExtensions
{
    public static WebApplicationBuilder UseHostingConfiguration(this WebApplicationBuilder builder)
    {
        builder.Host.UseDefaultServiceProvider(static (c, opts) =>
        {
            opts.ValidateScopes = true;
            opts.ValidateOnBuild = true;
        });

        return builder;
    }
}
