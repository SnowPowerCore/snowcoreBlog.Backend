using MaybeResults;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.Options;
using snowcoreBlog.Backend.AdminsManagement.Services;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Invites.Create;

public sealed class SendAdminInviteEmailStep(
    AdminEmailPublisher emailPublisher,
    IOptions<AdminsManagementOptions> moduleOptions)
    : IStep<CreateAdminInviteDelegate, CreateAdminInviteContext, IMaybe<AdminInviteDto>>
{
    public async Task<IMaybe<AdminInviteDto>> InvokeAsync(
        CreateAdminInviteContext context,
        CreateAdminInviteDelegate next,
        CancellationToken token = default)
    {
        var invite = context.Invite!;
        var acceptUrl = $"{ moduleOptions.Value.AdminFrontendDomain.TrimEnd('/') }/invite?token={ context.RawToken }";
        await emailPublisher.SendAsync(
            context.Email,
            "You have been invited as an administrator",
            $@"Hello,

            { context.InvitedByAdminEmail } invited you to become an administrator of snowcore's Blog.

            Open the link below to accept the invitation:
            { acceptUrl }

            The link is valid until {invite.ExpiresAtUtc:yyyy-MM-dd HH:mm} UTC.", CancellationToken.None);

        return Maybe.Create(new AdminInviteDto
        {
            Id = invite.Id,
            Email = invite.Email,
            NickName = invite.NickName,
            InvitedByAdminId = invite.InvitedByAdminId,
            CreatedAtUtc = invite.CreatedAtUtc,
            ExpiresAtUtc = invite.ExpiresAtUtc,
            AcceptedAtUtc = invite.AcceptedAtUtc,
        });
    }
}
