using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Marten;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.AdminsManagement.Services;
using snowcoreBlog.Backend.Core.Entities.Admin;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.AdminsManagement.Endpoints.Invites;

/// <summary>
/// Anonymous: resolves an invite token for the acceptance page (does NOT
/// consume the invite).
/// </summary>
public class ResolveAdminInviteEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public required IQuerySession QuerySession { get; set; }

    public override void Configure()
    {
        Get("invites/resolve");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.Administration)
            .Produces<ApiResponseForOpenApi<AdminInviteResolutionDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.NotFound, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var token = Query<string?>("token");
        var tokenHash = string.IsNullOrEmpty(token) ? string.Empty : AdminTokenUtilities.Sha256Hex(token);

        var invite = await QuerySession.Query<AdminInviteEntity>()
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);

        if (invite is default(AdminInviteEntity) || !invite.IsUsable)
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(
                    ["This invitation is invalid, already used, or expired."],
                    (int)HttpStatusCode.NotFound),
                (int)HttpStatusCode.NotFound,
                ct);
            return;
        }

        var result = Maybe.Create(new AdminInviteResolutionDto
        {
            Email = invite.Email,
            NickName = invite.NickName,
            InvitedByAdminId = invite.InvitedByAdminId,
            ExpiresAtUtc = invite.ExpiresAtUtc,
        });

        await Send.ResponseAsync(
            result.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result.ToStatusCode(),
            ct);
    }
}
