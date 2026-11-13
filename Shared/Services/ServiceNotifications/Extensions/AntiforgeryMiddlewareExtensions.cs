using Microsoft.AspNetCore.Antiforgery;

namespace snowcoreBlog.Backend.ServiceNotifications.Extensions;

/// <summary>
/// Endpoint metadata marker replicating FastEndpoints' per-endpoint <c>EnableAntiforgery()</c>:
/// only endpoints carrying this marker are validated by <see cref="AntiforgeryMiddlewareExtensions.UseAntiforgeryOptInConfiguration"/>.
/// </summary>
public sealed class RequireAntiforgeryMetadata;

public static class AntiforgeryMiddlewareExtensions
{
    /// <summary>
    /// Validates the antiforgery token pair for endpoints marked with
    /// <see cref="RequireAntiforgeryMetadata"/> (FastEndpoints parity: the default
    /// <c>RequestVerificationToken</c> header + antiforgery cookie pair; rejection
    /// responds with a bare 400 and an empty body).
    /// </summary>
    public static WebApplication UseAntiforgeryOptInConfiguration(this WebApplication app)
    {
        app.Use(async (HttpContext context, RequestDelegate next) =>
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<RequireAntiforgeryMetadata>() is not null)
            {
                var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();
                try
                {
                    await antiforgery.ValidateRequestAsync(context);
                }
                catch (AntiforgeryValidationException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }
            }

            await next(context);
        });

        return app;
    }
}
