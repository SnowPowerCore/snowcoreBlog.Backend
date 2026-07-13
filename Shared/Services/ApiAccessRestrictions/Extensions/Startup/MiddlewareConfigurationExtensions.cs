using Microsoft.AspNetCore.CookiePolicy;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Extensions.Startup;

public static class MiddlewareConfigurationExtensions
{
    public static WebApplication UseMiddlewareConfiguration(this WebApplication app)
    {
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