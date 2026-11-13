using System.Net;
using System.Net.Mime;
using FastEndpoints;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.Backend.AdminsManagement.Services;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.AdminsManagement.Endpoints.Auth;

/// <summary>
/// Clears the admin auth cookie (stateless JWT — nothing to revoke
/// server-side in this prototype).
/// </summary>
public class AdminLogoutEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    protected AdminTokenPairIssuer TokenIssuer { get; } = default!;

    public override void Configure()
    {
        Post("logout");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.Administration)
            .Produces<ApiResponse>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        TokenIssuer.ClearCookie();

        var result = Maybe.Create(new AdminLogoutResultDto());

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
