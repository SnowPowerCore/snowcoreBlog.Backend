using MassTransit;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.Backend.ApiAccessRestrictions.Extensions;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;
using snowcoreBlog.Backend.Core.Entities.ApiAccessRestrictions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessRules.Delete;

public sealed class DeleteApiAccessRuleStep(
    IApiAccessRuleRepository repo,
    IPublishEndpoint publisher)
    : IStep<DeleteApiAccessRuleDelegate, DeleteApiAccessRuleContext, IMaybe<ApiAccessRuleDto>>
{
    public async Task<IMaybe<ApiAccessRuleDto>> InvokeAsync(
        DeleteApiAccessRuleContext context,
        DeleteApiAccessRuleDelegate next,
        CancellationToken token = default)
    {
        await repo.RemoveAsync(context.Rule!, token: token);
        await publisher.Publish(new ApiAccessRestrictionsChanged(DateTimeOffset.UtcNow), token);

        return Maybe.Create(context.Rule!.ToDto());
    }
}
