using MaybeResults;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.AdminsManagement.Options;
using snowcoreBlog.Backend.AdminsManagement.Services;
using snowcoreBlog.Backend.Core.Entities.Admin;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Invites.Create;

public sealed class CreateAdminInviteEntityStep(
    IAdminInviteRepository inviteRepository,
    IOptions<AdminsManagementOptions> moduleOptions)
    : IStep<CreateAdminInviteDelegate, CreateAdminInviteContext, IMaybe<AdminInviteDto>>
{
    public async Task<IMaybe<AdminInviteDto>> InvokeAsync(
        CreateAdminInviteContext context,
        CreateAdminInviteDelegate next,
        CancellationToken token = default)
    {
        var now = DateTime.UtcNow;
        var rawToken = AdminTokenUtilities.NewToken();
        context.RawToken = rawToken;

        var invite = new AdminInviteEntity
        {
            Email = context.Email,
            NickName = context.Request.NickName.Trim(),
            TokenHash = AdminTokenUtilities.Sha256Hex(rawToken),
            InvitedByAdminId = context.InvitedByAdminId,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddDays(moduleOptions.Value.InviteValidityDurationInDays),
        };

        context.Invite = await inviteRepository.AddOrUpdateAsync(invite, token: token);

        return await next(context, token);
    }
}
