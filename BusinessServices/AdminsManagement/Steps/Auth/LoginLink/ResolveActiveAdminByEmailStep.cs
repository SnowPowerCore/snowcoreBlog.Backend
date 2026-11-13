using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.ErrorResults;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Auth.LoginLink;

public sealed class ResolveActiveAdminByEmailStep(IAdminRepository adminRepository)
    : IStep<RequestAdminLoginLinkDelegate, RequestAdminLoginLinkContext, IMaybe<RequestAdminLoginLinkResultDto>>
{
    public async Task<IMaybe<RequestAdminLoginLinkResultDto>> InvokeAsync(
        RequestAdminLoginLinkContext context,
        RequestAdminLoginLinkDelegate next,
        CancellationToken token = default)
    {
        var admin = await adminRepository.GetActiveByEmailAsync(context.Email, token);

        if (admin is default(Core.Entities.Admin.AdminEntity))
        {
            return AdminNotExistsOrInactiveError<RequestAdminLoginLinkResultDto>.Create(
                "This admin account no longer exists or is inactive.");
        }

        context.Admin = admin;

        return await next(context, token);
    }
}
