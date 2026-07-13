using Microsoft.AspNetCore.CookiePolicy;
using MinimalStepifiedSystem.Extensions;

namespace snowcoreBlog.Backend.Articles.Extensions.Startup;

public static class MiddlewareConfigurationExtensions
{
    public static WebApplication UseMiddlewareConfiguration(this WebApplication app)
    {
        app.UseStepifiedSystem();
        app.UseHttpsRedirection()
            .UseCookiePolicy(new()
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
                HttpOnly = HttpOnlyPolicy.Always,
                Secure = CookieSecurePolicy.Always
            });

        return app;
    }
}