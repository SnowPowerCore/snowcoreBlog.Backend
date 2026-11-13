using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Admins.Delete;

public sealed class SoftDeleteAdminStep(IAdminRepository adminRepository)
    : IStep<DeleteAdminAccountDelegate, DeleteAdminAccountContext, IMaybe<DeleteAdminAccountResultDto>>
{
    public async Task<IMaybe<DeleteAdminAccountResultDto>> InvokeAsync(
        DeleteAdminAccountContext context,
        DeleteAdminAccountDelegate next,
        CancellationToken token = default)
    {
        var admin = context.Admin!;

        await adminRepository.RemoveAsync(admin, token: token);

        return Maybe.Create(new DeleteAdminAccountResultDto());
    }
}
