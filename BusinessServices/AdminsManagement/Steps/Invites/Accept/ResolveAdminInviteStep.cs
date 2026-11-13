using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.ErrorResults;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.AdminsManagement.Services;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Invites.Accept;

public sealed class ResolveAdminInviteStep(IAdminInviteRepository inviteRepository)
    : IStep<AcceptAdminInviteDelegate, AcceptAdminInviteContext, IMaybe<AdminDto>>
{
    public async Task<IMaybe<AdminDto>> InvokeAsync(
        AcceptAdminInviteContext context,
        AcceptAdminInviteDelegate next,
        CancellationToken token = default)
    {
        var tokenHash = AdminTokenUtilities.Sha256Hex(context.Token);

        context.ClearSensitiveData();

        var invite = await inviteRepository.GetByTokenHashAsync(tokenHash, token);
        if (invite is default(Core.Entities.Admin.AdminInviteEntity) || !invite.IsUsable)
        {
            return InviteNotFoundOrNotUsableError<AdminDto>.Create(
                "This invitation is invalid, already used, or expired.");
        }

        context.Invite = invite;
        context.EffectiveNickName = string.IsNullOrWhiteSpace(context.RequestedNickName)
            ? invite.NickName
            : context.RequestedNickName.Trim();

        return await next(context, token);
    }
}
