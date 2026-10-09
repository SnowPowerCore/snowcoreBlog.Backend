using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.Extensions.Options;
using AspNet.Security.OAuth.Apple;
using snowcoreBlog.Backend.ReadersManagement.Options;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.External;

/// <summary>
/// Starts the external-provider (Microsoft / Google / Apple) login flow by
/// redirecting the browser to the provider. The provider redirects the browser
/// back to the matching scheme CallbackPath, where the remote authentication
/// handler — wired in ExternalAuthenticationConfigurationExtensions — completes
/// the sign-in and redirects to the frontend with the auth cookies in place.
/// </summary>
public class ExternalLoginChallengeEndpoint(IOptions<ExternalAuthenticationOptions> externalOptions) : EndpointWithoutRequest
{
    private static readonly Dictionary<string, Func<ExternalAuthenticationOptions, bool>> ProviderSchemeConfigured = new(StringComparer.OrdinalIgnoreCase)
    {
        ["microsoft"] = options => options.Microsoft.IsConfigured,
        ["google"] = options => options.Google.IsConfigured,
        ["apple"] = options => options.Apple.IsConfigured,
    };

    private static readonly Dictionary<string, string> ProviderSchemes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["microsoft"] = MicrosoftAccountDefaults.AuthenticationScheme,
        ["google"] = GoogleDefaults.AuthenticationScheme,
        ["apple"] = AppleAuthenticationDefaults.AuthenticationScheme,
    };

    public override void Configure()
    {
        Get("login/external/{provider}");
        Version(1);
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.ReaderAccountManagement)
            .Produces((int)HttpStatusCode.Redirect)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var provider = Route<string>("provider") ?? string.Empty;

        if (!ProviderSchemeConfigured.TryGetValue(provider, out var isConfigured) ||
            !isConfigured(externalOptions.Value))
        {
            AddError($"External login provider [{provider}] is not available.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        await HttpContext.ChallengeAsync(ProviderSchemes[provider], new AuthenticationProperties());
    }
}
