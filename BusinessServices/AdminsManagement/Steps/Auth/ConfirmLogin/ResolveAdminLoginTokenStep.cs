using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.ErrorResults;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.AdminsManagement.Services;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Auth.ConfirmLogin;

public sealed class ResolveAdminLoginTokenStep(IAdminLoginTokenRepository loginTokenRepository)
    : IStep<ConfirmAdminLoginDelegate, ConfirmAdminLoginContext, IMaybe<AdminDto>>
{
    public async Task<IMaybe<AdminDto>> InvokeAsync(
        ConfirmAdminLoginContext context,
        ConfirmAdminLoginDelegate next,
        CancellationToken token = default)
    {
        var tokenHash = AdminTokenUtilities.Sha256Hex(context.Token);
        var loginToken = await loginTokenRepository.GetByTokenHashAsync(tokenHash, token);

        if (loginToken is default(Core.Entities.Admin.AdminLoginTokenEntity) || !loginToken.IsUsable)
        {
            return AdminInvalidLoginTokenError<AdminDto>.Create(
                "This sign-in link is invalid, already used, or expired.");
        }

        context.LoginToken = loginToken;

        return await next(context, token);
    }
}
