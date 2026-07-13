using snowcoreBlog.ServiceDefaults.Extensions;

namespace snowcoreBlog.Backend.IAM.Extensions.Startup;

public static class ApplicationConfigurationExtensions
{
    public static WebApplication UseApplicationConfiguration(this WebApplication app)
    {
        app.UseHttpsRedirection();

        return app;
    }

    public static void MapEndpoints(this WebApplication app)
    {
        app.MapDefaultEndpoints();
    }

    public static async Task RunApplicationAsync(this WebApplication app)
    {
        await app.RunAsync();
    }
}