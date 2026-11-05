using System.Net;
using System.Net.Mime;
using FastEndpoints;
using MassTransit;
using Marten;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.AdminsManagement.Services;
using snowcoreBlog.Backend.Core.Entities.Admin;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.AdminsManagement.Endpoints.Invites;

/// <summary>
/// Anonymous (token-gated): accepts a pending invite — creates the admin
/// entity with the STRONG reference to the inviting admin, consumes the
/// invite and signs the new admin in (admin auth cookie).
/// </summary>
public class AcceptAdminInviteEndpoint : Endpoint<AcceptAdminInviteDto, ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public required IDocumentSession DocumentSession { get; set; }

    public required IRequestClient<CreateAdmin> CreateAdminClient { get; set; }

    public required AdminTokenPairIssuer TokenIssuer { get; set; }

    public override void Configure()
    {
        Post("invites/accept");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.Administration)
            .Accepts<AcceptAdminInviteDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<AdminDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.NotFound, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(AcceptAdminInviteDto req, CancellationToken ct)
    {
        var tokenHash = AdminTokenUtilities.Sha256Hex(req.Token);

        var invite = await DocumentSession.Query<AdminInviteEntity>()
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

        var nickName = string.IsNullOrWhiteSpace(req.NickName) ? invite.NickName : req.NickName.Trim();

        // Create the IAM identity FIRST — the Marten admin row references it.
        // FromAdmin = the inviting admin (must exist in IAM; every admin has an
        // IAM identity since this wiring, incl. the bootstrap admin).
        var iamResponse = await CreateAdminClient.GetResponse<DataResult<AdminCreationResult>>(
            new CreateAdmin
            {
                FromAdmin = invite.InvitedByAdminId,
                Email = invite.Email,
                NickName = nickName,
            }, ct);

        if (!iamResponse.Message.IsSuccess || iamResponse.Message.Value is default(AdminCreationResult))
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(
                    iamResponse.Message.Errors?.Select(static e => $"{e.Code}: {e.Description}").ToList()
                        ?? ["Failed to create the admin identity in IAM."],
                    (int)HttpStatusCode.Conflict),
                (int)HttpStatusCode.Conflict,
                ct);
            return;
        }

        var admin = new AdminEntity
        {
            Email = invite.Email,
            NickName = nickName,
            // STRONG inviter reference — non-null for every invited admin.
            InvitedByAdminId = invite.InvitedByAdminId,
            IamUserId = iamResponse.Message.Value.Id,
            InvitedAtUtc = DateTime.UtcNow,
            IsActive = true,
        };

        DocumentSession.Insert(admin);
        invite = invite with { AcceptedAtUtc = DateTime.UtcNow };
        DocumentSession.Update(invite);
        await DocumentSession.SaveChangesAsync(ct);

        TokenIssuer.IssueAndSetCookie(admin);

        var result = Maybe.Create(admin.ToDto());

        await Send.ResponseAsync(
            result.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result.ToStatusCode(),
            ct);
    }
}
