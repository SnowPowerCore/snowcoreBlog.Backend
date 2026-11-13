using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.ErrorResults;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Auth.ConfirmLogin;

public sealed class ResolveAdminForLoginStep(IAdminRepository adminRepository)
    : IStep<ConfirmAdminLoginDelegate, ConfirmAdminLoginContext, IMaybe<AdminDto>>
{
    public async Task<IMaybe<AdminDto>> InvokeAsync(
        ConfirmAdminLoginContext context,
        ConfirmAdminLoginDelegate next,
        CancellationToken token = default)
    {
        var admin = await adminRepository.GetActiveByEmailAsync(context.LoginToken!.Email, token);

        if (admin is default(Core.Entities.Admin.AdminEntity))
        {
            return AdminNotExistsOrInactiveError<AdminDto>.Create(
                "This admin account no longer exists or is inactive.");
        }

        context.Admin = admin;

        return await next(context, token);
    }
}
