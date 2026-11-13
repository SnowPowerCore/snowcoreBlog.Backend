using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.ErrorResults;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Invites.Create;

public sealed class ValidateNoOpenInviteStep(IAdminInviteRepository inviteRepository)
    : IStep<CreateAdminInviteDelegate, CreateAdminInviteContext, IMaybe<AdminInviteDto>>
{
    public async Task<IMaybe<AdminInviteDto>> InvokeAsync(
        CreateAdminInviteContext context,
        CreateAdminInviteDelegate next,
        CancellationToken token = default)
    {
        if (await inviteRepository.AnyUsableByEmailAsync(context.Email, token))
        {
            return AdminInviteExistsByEmailError<AdminInviteDto>.Create(
                $"An open invite for '{ context.Email }' already exists.");
        }

        return await next(context, token);
    }
}
