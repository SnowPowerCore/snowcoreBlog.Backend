using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Marten;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.AdminsManagement.Constants;
using snowcoreBlog.Backend.AdminsManagement.Options;
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
/// Admin: invites a new admin. Creates a single-use tokenized invite and
/// emails the acceptance link. Admins CANNOT self-register — this is the
/// only path to a new admin account.
/// </summary>
public class CreateAdminInviteEndpoint : Endpoint<CreateAdminInviteDto, ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public required IDocumentSession DocumentSession { get; set; }

    public required AdminEmailPublisher EmailPublisher { get; set; }

    public required IOptions<AdminsManagementOptions> ModuleOptions { get; set; }

    public override void Configure()
    {
        Post("invites");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Claims(AdminClaimConstants.AdminAccountClaimKey);
        Description(b => b
            .WithTags(ApiTagConstants.Administration)
            .Accepts<CreateAdminInviteDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<AdminInviteDto>>((int)HttpStatusCode.Created, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.Conflict, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CreateAdminInviteDto req, CancellationToken ct)
    {
        var email = req.Email.Trim().ToLowerInvariant();
        if (email.Length == 0 || !email.Contains('@'))
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(["A valid email address is required."], (int)HttpStatusCode.BadRequest),
                (int)HttpStatusCode.BadRequest,
                ct);
            return;
        }

        if (await DocumentSession.Query<AdminEntity>().AnyAsync(x =>
            x.Email.Equals(email, StringComparison.OrdinalIgnoreCase), ct))
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors([$"An admin with email '{email}' already exists."], (int)HttpStatusCode.Conflict),
                (int)HttpStatusCode.Conflict,
                ct);
            return;
        }

        if (await DocumentSession.Query<AdminInviteEntity>().AnyAsync(x =>
            x.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && x.IsUsable, ct))
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors([$"An open invite for '{email}' already exists."], (int)HttpStatusCode.Conflict),
                (int)HttpStatusCode.Conflict,
                ct);
            return;
        }

        var rawToken = AdminEmailPublisher.NewToken();
        var invite = new AdminInviteEntity
        {
            Email = email,
            NickName = req.NickName.Trim(),
            TokenHash = AdminEmailPublisher.Sha256Hex(rawToken),
            InvitedByAdminId = User.GetAdminId(),
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(ModuleOptions.Value.InviteValidityDurationInDays),
        };

        DocumentSession.Insert(invite);
        await DocumentSession.SaveChangesAsync(ct);

        var acceptUrl = $"{ModuleOptions.Value.AdminFrontendDomain.TrimEnd('/')}/invite?token={rawToken}";
        await EmailPublisher.SendAsync(
            email,
            "You have been invited as an administrator",
            $@"Hello,

            {User.GetAdminEmail()} invited you to become an administrator of snowcore's Blog.

            Open the link below to accept the invitation:
            {acceptUrl}

            The link is valid until {invite.ExpiresAtUtc:yyyy-MM-dd HH:mm} UTC.", CancellationToken.None);

        var result = Maybe.Create(new AdminInviteDto
        {
            Id = invite.Id,
            Email = invite.Email,
            NickName = invite.NickName,
            InvitedByAdminId = invite.InvitedByAdminId,
            CreatedAtUtc = invite.CreatedAtUtc,
            ExpiresAtUtc = invite.ExpiresAtUtc,
            AcceptedAtUtc = invite.AcceptedAtUtc,
        });

        await Send.ResponseAsync(
            result.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            (int)HttpStatusCode.Created,
            ct);
    }
}
