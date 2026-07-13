using FastEndpoints.OpenTelemetry.Middleware;
using Microsoft.AspNetCore.CookiePolicy;
using snowcoreBlog.Backend.Infrastructure.Middleware;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions.Startup;

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
            })
            .UseAuthentication()
            .UseAuthorization();

        return app;
    }

    public static WebApplication UseCustomMiddlewareConfiguration(this WebApplication app)
    {
        app.UseMiddleware<UserCookieJsonWebTokenMiddleware>()
            .UseFastEndpointsDiagnosticsMiddleware();

        return app;
    }
}