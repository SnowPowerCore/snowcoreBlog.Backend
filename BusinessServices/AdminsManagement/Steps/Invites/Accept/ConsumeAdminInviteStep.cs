using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Invites.Accept;

public sealed class ConsumeAdminInviteStep(
    IAdminInviteRepository inviteRepository,
    IAdminRepository adminRepository)
    : IStep<AcceptAdminInviteDelegate, AcceptAdminInviteContext, IMaybe<AdminDto>>
{
    public async Task<IMaybe<AdminDto>> InvokeAsync(
        AcceptAdminInviteContext context,
        AcceptAdminInviteDelegate next,
        CancellationToken token = default)
    {
        var invite = context.Invite! with { AcceptedAtUtc = DateTime.UtcNow };
        await inviteRepository.AddOrUpdateAsync(invite, saveChange: false, token: token);
        context.Invite = invite;

        // Persist admin entity + consumed invite together.
        await adminRepository.AddOrUpdateAsync(context.Admin!, token: token);

        return await next(context, token);
    }
}
