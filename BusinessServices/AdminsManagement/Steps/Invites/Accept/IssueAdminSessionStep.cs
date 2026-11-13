using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.Extensions;
using snowcoreBlog.Backend.AdminsManagement.Services;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Steps.Invites.Accept;

public sealed class IssueAdminSessionStep(
    AdminTokenPairIssuer tokenIssuer)
    : IStep<AcceptAdminInviteDelegate, AcceptAdminInviteContext, IMaybe<AdminDto>>
{
    public Task<IMaybe<AdminDto>> InvokeAsync(
        AcceptAdminInviteContext context,
        AcceptAdminInviteDelegate next,
        CancellationToken token = default)
    {
        tokenIssuer.IssueAndSetCookie(context.Admin!);

        return Task.FromResult(Maybe.Create(context.Admin!.ToDto()));
    }
}
