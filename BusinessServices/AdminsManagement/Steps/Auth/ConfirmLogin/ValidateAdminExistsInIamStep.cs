using MassTransit;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.ErrorResults;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Auth.ConfirmLogin;

public sealed class ValidateAdminExistsInIamStep(IRequestClient<ValidateAdminExists> validateAdminClient)
    : IStep<ConfirmAdminLoginDelegate, ConfirmAdminLoginContext, IMaybe<AdminDto>>
{
    public async Task<IMaybe<AdminDto>> InvokeAsync(
        ConfirmAdminLoginContext context,
        ConfirmAdminLoginDelegate next,
        CancellationToken token = default)
    {
        // Double-check the identity still exists in IAM before issuing a session.
        var iamCheck = await validateAdminClient.GetResponse<DataResult<AdminExistsValidationResult>>(
            new ValidateAdminExists { Email = context.LoginToken!.Email }, token);

        if (!iamCheck.Message.IsSuccess || iamCheck.Message.Value?.Exists is false)
        {
            return AdminApplicationAdminNotExistsError<AdminDto>.Create(
                "This admin identity no longer exists in IAM.");
        }

        return await next(context, token);
    }
}
