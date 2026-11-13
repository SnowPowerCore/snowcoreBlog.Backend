using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.ErrorResults;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Admins.Delete;

public sealed class ValidateNotLastActiveAdminStep(IAdminRepository adminRepository)
    : IStep<DeleteAdminAccountDelegate, DeleteAdminAccountContext, IMaybe<DeleteAdminAccountResultDto>>
{
    public async Task<IMaybe<DeleteAdminAccountResultDto>> InvokeAsync(
        DeleteAdminAccountContext context,
        DeleteAdminAccountDelegate next,
        CancellationToken token = default)
    {
        // Enforce at least one remaining active admin.
        var activeAdmins = await adminRepository.GetActiveExceptAsync(context.TargetId, token);
        if (context.IsActive && activeAdmins.Count == 0)
        {
            return AdminLastActiveAdminError<DeleteAdminAccountResultDto>.Create(
                "The last active admin cannot be deleted.");
        }

        return await next(context, token);
    }
}
