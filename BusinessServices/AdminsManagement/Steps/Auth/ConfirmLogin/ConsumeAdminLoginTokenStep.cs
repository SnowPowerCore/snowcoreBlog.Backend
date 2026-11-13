using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Auth.ConfirmLogin;

public sealed class ConsumeAdminLoginTokenStep(IAdminLoginTokenRepository loginTokenRepository)
    : IStep<ConfirmAdminLoginDelegate, ConfirmAdminLoginContext, IMaybe<AdminDto>>
{
    public async Task<IMaybe<AdminDto>> InvokeAsync(
        ConfirmAdminLoginContext context,
        ConfirmAdminLoginDelegate next,
        CancellationToken token = default)
    {
        // Single-use: mark the login token as consumed.
        var loginToken = context.LoginToken! with { UsedAtUtc = DateTime.UtcNow };

        await loginTokenRepository.AddOrUpdateAsync(loginToken, loginToken.Id, token: token);

        context.LoginToken = loginToken;

        return await next(context, token);
    }
}
