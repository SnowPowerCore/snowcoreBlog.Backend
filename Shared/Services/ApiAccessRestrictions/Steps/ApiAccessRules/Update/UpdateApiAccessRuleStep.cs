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

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessRules.Update;

public sealed class UpdateApiAccessRuleStep(
    IApiAccessRuleRepository repo,
    IPublishEndpoint publisher)
    : IStep<UpdateApiAccessRuleDelegate, UpdateApiAccessRuleContext, IMaybe<ApiAccessRuleDto>>
{
    public async Task<IMaybe<ApiAccessRuleDto>> InvokeAsync(
        UpdateApiAccessRuleContext context,
        UpdateApiAccessRuleDelegate next,
        CancellationToken token = default)
    {
        context.Request.ApplyTo(context.Rule!);

        var stored = (ApiAccessRuleEntity)await repo.AddOrUpdateAsync(context.Rule!, context.Rule!.Id, token: token);
        await publisher.Publish(new ApiAccessRestrictionsChanged(DateTimeOffset.UtcNow), token);

        return Maybe.Create(stored.ToDto());
    }
}
