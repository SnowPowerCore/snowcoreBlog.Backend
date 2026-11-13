using MaybeResults;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.Backend.AdminsManagement.Options;
using snowcoreBlog.Backend.AdminsManagement.Services;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Auth.LoginLink;

public sealed class SendAdminLoginLinkEmailStep(
    AdminEmailPublisher emailPublisher,
    IOptions<AdminsManagementOptions> moduleOptions)
    : IStep<RequestAdminLoginLinkDelegate, RequestAdminLoginLinkContext, IMaybe<RequestAdminLoginLinkResultDto>>
{
    public async Task<IMaybe<RequestAdminLoginLinkResultDto>> InvokeAsync(
        RequestAdminLoginLinkContext context,
        RequestAdminLoginLinkDelegate next,
        CancellationToken token = default)
    {
        var loginUrl = $"{moduleOptions.Value.AdminFrontendDomain.TrimEnd('/')}/login?token={context.RawToken}";
        await emailPublisher.SendAsync(
            context.Email,
            "Your admin sign-in link",
            $@"Hello,

            Use the link below to sign in to the snowcore's Blog admin portal:

            { loginUrl }

            The link is single-use and valid for { moduleOptions.Value.LoginLinkValidityDurationInMinutes } minutes.
            If you did not request it, you can safely ignore this email.", CancellationToken.None);

        return Maybe.Create(new RequestAdminLoginLinkResultDto());
    }
}
