using System.Net;
using System.Net.Mime;
using FastEndpoints;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.AdminsManagement.ErrorResults;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.AdminsManagement.Services;
using snowcoreBlog.Backend.AdminsManagement.Core.Entities.Admin;
using snowcoreBlog.Backend.Infrastructure;
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

    protected IAdminInviteRepository InviteRepository { get; } = default!;

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
        IMaybe<AdminInviteResolutionDto> result;
        var token = Query<string?>("token");
        var tokenHash = string.IsNullOrEmpty(token) ? string.Empty : AdminTokenUtilities.Sha256Hex(token);

        var invite = await InviteRepository.GetByTokenHashAsync(tokenHash, ct);
        if (invite is { IsUsable: true })
        {
            result = Maybe.Create(new AdminInviteResolutionDto
            {
                Email = invite.Email,
                NickName = invite.NickName,
                InvitedByAdminId = invite.InvitedByAdminId,
                ExpiresAtUtc = invite.ExpiresAtUtc,
            });
        }
        else
        {
            result = InviteNotFoundOrNotUsableError<AdminInviteResolutionDto>.Create(
                "This invitation is invalid, already used, or expired.");
        }

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
