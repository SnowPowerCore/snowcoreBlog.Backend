using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.ErrorResults;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Admins.Delete;

public sealed class ValidateAdminExistsForDeleteStep(IAdminRepository adminRepository)
    : IStep<DeleteAdminAccountDelegate, DeleteAdminAccountContext, IMaybe<DeleteAdminAccountResultDto>>
{
    public async Task<IMaybe<DeleteAdminAccountResultDto>> InvokeAsync(
        DeleteAdminAccountContext context,
        DeleteAdminAccountDelegate next,
        CancellationToken token = default)
    {
        var admin = await adminRepository.GetByIdAsync(context.TargetId, token);
        if (admin is default(Core.Entities.Admin.AdminEntity))
        {
            return AdminNotExistsError<DeleteAdminAccountResultDto>.Create(
                $"Admin account with ID '{ context.TargetId }' was not found.");
        }

        context.Admin = admin;

        return await next(context, token);
    }
}
