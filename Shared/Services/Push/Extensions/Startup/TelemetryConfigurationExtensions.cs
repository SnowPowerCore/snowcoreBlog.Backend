using snowcoreBlog.ServiceDefaults.Extensions;

namespace snowcoreBlog.Backend.Push.Extensions.Startup;

public static class TelemetryConfigurationExtensions
{
    public static WebApplicationBuilder UseServiceDefaultsConfiguration(this WebApplicationBuilder builder)
    {
        builder.WebHost.UseKestrelHttpsConfiguration();
        builder.AddServiceDefaults();

        return builder;
    }
}
