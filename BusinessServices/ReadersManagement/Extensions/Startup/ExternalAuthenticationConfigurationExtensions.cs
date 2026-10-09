using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using AspNet.Security.OAuth.Apple;
using snowcoreBlog.Backend.ReadersManagement.Options;
using snowcoreBlog.Backend.ReadersManagement.Services;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions.Startup;

public static class ExternalAuthenticationConfigurationExtensions
{
    public const string ExternalLoginErrorRoute = "/login";
    public const string ExternalLoginErrorQueryKey = "externalError";

    /// <summary>
    /// Registers remote authentication schemes for the external identity
    /// providers. A scheme is registered only when its credentials are present
    /// in configuration (<c>Integrations:ExternalAuth</c>), so local development
    /// without provider secrets keeps working.
    /// <para>
    /// The provider redirects the browser back to the scheme CallbackPath
    /// (behind the gateway: <c>/api/readers/login/external/&#123;provider&#125;/callback</c>);
    /// the OnTicketReceived event completes the sign-in by resolving the reader
    /// account for the provider-verified email and issuing the JWT pair, then
    /// redirects the browser to the frontend.
    /// </para>
    /// </summary>
    public static IServiceCollection AddExternalAuthenticationConfiguration(
        this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("Integrations:ExternalAuth");
        services.Configure<ExternalAuthenticationOptions>(section);

        var options = section.Get<ExternalAuthenticationOptions>() ?? new ExternalAuthenticationOptions();
        var auth = services.AddAuthentication();
        var frontendDomain = configuration["Project:PublicFrontendDomain"] ?? "https://localhost/app";

        if (options.Microsoft.IsConfigured)
        {
            auth.AddMicrosoftAccount((o) =>
            {
                var microsoft = section.Get<ExternalAuthenticationOptions>()!.Microsoft;
                o.ClientId = microsoft.ClientId;
                o.ClientSecret = microsoft.ClientSecret;
                o.CallbackPath = "/login/external/microsoft/callback";
                o.Events.OnTicketReceived += CompleteExternalSignInDelegate(frontendDomain);
                o.Events.OnRemoteFailure += RedirectToFrontendErrorDelegate(frontendDomain);
            });
        }

        if (options.Google.IsConfigured)
        {
            auth.AddGoogle((o) =>
            {
                var google = section.Get<ExternalAuthenticationOptions>()!.Google;
                o.ClientId = google.ClientId;
                o.ClientSecret = google.ClientSecret;
                o.CallbackPath = "/login/external/google/callback";
                o.Events.OnTicketReceived += CompleteExternalSignInDelegate(frontendDomain);
                o.Events.OnRemoteFailure += RedirectToFrontendErrorDelegate(frontendDomain);
            });
        }

        if (options.Apple.IsConfigured)
        {
            auth.AddApple((o) =>
            {
                var apple = section.Get<ExternalAuthenticationOptions>()!.Apple;
                o.ClientId = apple.ClientId;
                o.KeyId = apple.KeyId;
                o.TeamId = apple.TeamId;
                o.CallbackPath = "/login/external/apple/callback";
                o.PrivateKey = (_, _) => Task.FromResult(new ReadOnlyMemory<char>(apple.PrivateKeyPem.ToCharArray()));
                o.Events.OnTicketReceived += CompleteExternalSignInDelegate(frontendDomain);
                o.Events.OnRemoteFailure += RedirectToFrontendErrorDelegate(frontendDomain);
            });
        }

        return services;
    }

    private static Func<TicketReceivedContext, Task> CompleteExternalSignInDelegate(string frontendDomain) =>
        async (context) =>
        {
            // Suppress the default ticket handling (cookie sign-in) — this service
            // is headless; completion is a redirect with the JWT pair cookies set.
            context.HandleResponse();

            var email = ExtractProviderEmail(context.Principal);
            if (string.IsNullOrWhiteSpace(email))
            {
                context.Response.Redirect($"{frontendDomain}{ExternalLoginErrorRoute}?{ExternalLoginErrorQueryKey}=no-email");
                return;
            }

            var signIn = context.HttpContext.RequestServices.GetRequiredService<ExternalReaderAccountSignInService>();
            var success = await signIn.TrySignInAsync(email, context.HttpContext.RequestAborted);

            context.Response.Redirect(success
                ? $"{frontendDomain}/"
                : $"{frontendDomain}{ExternalLoginErrorRoute}?{ExternalLoginErrorQueryKey}=no-account");
        };

    private static Func<RemoteFailureContext, Task> RedirectToFrontendErrorDelegate(string frontendDomain) =>
        (context) =>
        {
            context.HandleResponse();
            context.Response.Redirect($"{frontendDomain}{ExternalLoginErrorRoute}?{ExternalLoginErrorQueryKey}=provider");
            return Task.CompletedTask;
        };

    private static string? ExtractProviderEmail(System.Security.Claims.ClaimsPrincipal? principal) =>
        principal?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ??
        principal?.FindFirst("email")?.Value ??
        (principal?.FindFirst("preferred_username")?.Value is { } userName && userName.Contains('@') ? userName : null);
}
