using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Core.Entities.Admin;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Invites.Accept;

public sealed class CreateAdminEntityStep(IAdminRepository adminRepository)
    : IStep<AcceptAdminInviteDelegate, AcceptAdminInviteContext, IMaybe<AdminDto>>
{
    public async Task<IMaybe<AdminDto>> InvokeAsync(
        AcceptAdminInviteContext context,
        AcceptAdminInviteDelegate next,
        CancellationToken token = default)
    {
        var admin = new AdminEntity
        {
            Email = context.Invite!.Email,
            NickName = context.EffectiveNickName,
            // STRONG inviter reference — non-null for every invited admin.
            InvitedByAdminId = context.Invite.InvitedByAdminId,
            IamUserId = context.CreatedIamUserId,
            InvitedAtUtc = DateTime.UtcNow,
            IsActive = true,
        };

        context.Admin = await adminRepository.AddOrUpdateAsync(admin, saveChange: false, token: token);

        return await next(context, token);
    }
}
