using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.ErrorResults;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Admins.Delete;

public sealed class ValidateNotDeletingSelfStep
    : IStep<DeleteAdminAccountDelegate, DeleteAdminAccountContext, IMaybe<DeleteAdminAccountResultDto>>
{
    public Task<IMaybe<DeleteAdminAccountResultDto>> InvokeAsync(
        DeleteAdminAccountContext context,
        DeleteAdminAccountDelegate next,
        CancellationToken token = default)
    {
        if (context.TargetId == context.CurrentAdminId)
        {
            return Task.FromResult(AdminCannotDeleteSelfError<DeleteAdminAccountResultDto>.Create(
                "You cannot delete your own admin account."));
        }

        return next(context, token);
    }
}
