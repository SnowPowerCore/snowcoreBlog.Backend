using MaybeResults;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.Backend.AdminsManagement.Options;
using snowcoreBlog.Backend.AdminsManagement.Services;
using snowcoreBlog.Backend.AdminsManagement.Core.Entities.Admin;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Auth.LoginLink;

public sealed class CreateAdminLoginTokenStep(
    IAdminLoginTokenRepository loginTokenRepository,
    IOptions<AdminsManagementOptions> moduleOptions)
    : IStep<RequestAdminLoginLinkDelegate, RequestAdminLoginLinkContext, IMaybe<RequestAdminLoginLinkResultDto>>
{
    public async Task<IMaybe<RequestAdminLoginLinkResultDto>> InvokeAsync(
        RequestAdminLoginLinkContext context,
        RequestAdminLoginLinkDelegate next,
        CancellationToken token = default)
    {
        var now = DateTime.UtcNow;
        var rawToken = AdminTokenUtilities.NewToken();
        context.RawToken = rawToken;

        await loginTokenRepository.AddOrUpdateAsync(new AdminLoginTokenEntity
        {
            Email = context.Email,
            TokenHash = AdminTokenUtilities.Sha256Hex(rawToken),
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddMinutes(moduleOptions.Value.LoginLinkValidityDurationInMinutes),
        }, token: token);

        return await next(context, token);
    }
}
