using MassTransit;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.ErrorResults;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Invites.Accept;

public sealed class CreateAdminIdentityStep(IRequestClient<CreateAdmin> createAdminClient)
    : IStep<AcceptAdminInviteDelegate, AcceptAdminInviteContext, IMaybe<AdminDto>>
{
    public async Task<IMaybe<AdminDto>> InvokeAsync(
        AcceptAdminInviteContext context,
        AcceptAdminInviteDelegate next,
        CancellationToken token = default)
    {
        // Create the IAM identity FIRST — the Marten admin row references it.
        // FromAdmin = the inviting admin (must exist in IAM; every admin has an
        // IAM identity since this wiring, incl. the bootstrap admin).
        var iamResponse = await createAdminClient.GetResponse<DataResult<AdminCreationResult>>(
            new CreateAdmin
            {
                FromAdmin = context.Invite!.InvitedByAdminId,
                Email = context.Invite.Email,
                NickName = context.EffectiveNickName,
            }, token);

        if (!iamResponse.Message.IsSuccess || iamResponse.Message.Value is default(AdminCreationResult))
        {
            return InviteCreateApplicationAdminError<AdminDto>.Create(
                "Failed to create the admin identity in IAM.");
        }

        context.CreatedIamUserId = iamResponse.Message.Value.Id;
        return await next(context, token);
    }
}
