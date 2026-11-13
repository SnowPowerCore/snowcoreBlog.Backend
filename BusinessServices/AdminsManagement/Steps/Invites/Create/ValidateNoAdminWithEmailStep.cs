using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.ErrorResults;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Invites.Create;

public sealed class ValidateNoAdminWithEmailStep(IAdminRepository adminRepository)
    : IStep<CreateAdminInviteDelegate, CreateAdminInviteContext, IMaybe<AdminInviteDto>>
{
    public async Task<IMaybe<AdminInviteDto>> InvokeAsync(
        CreateAdminInviteContext context,
        CreateAdminInviteDelegate next,
        CancellationToken token = default)
    {
        if (await adminRepository.AnyByEmailAsync(context.Email, token))
        {
            return AdminAlreadyExistsByEmailError<AdminInviteDto>.Create(
                $"An admin with email '{ context.Email }' already exists.");
        }

        return await next(context, token);
    }
}
