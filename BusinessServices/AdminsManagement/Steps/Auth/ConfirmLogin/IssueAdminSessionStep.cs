using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.Extensions;
using snowcoreBlog.Backend.AdminsManagement.Services;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Auth.ConfirmLogin;

public sealed class IssueAdminSessionStep(AdminTokenPairIssuer tokenIssuer)
    : IStep<ConfirmAdminLoginDelegate, ConfirmAdminLoginContext, IMaybe<AdminDto>>
{
    public async Task<IMaybe<AdminDto>> InvokeAsync(
        ConfirmAdminLoginContext context,
        ConfirmAdminLoginDelegate next,
        CancellationToken token = default)
    {
        tokenIssuer.IssueAndSetCookie(context.Admin!);

        return Maybe.Create(context.Admin!.ToDto());
    }
}
