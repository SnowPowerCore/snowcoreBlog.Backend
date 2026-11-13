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

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessRules.Create;

public sealed class CreateApiAccessRuleStep(
    IApiAccessRuleRepository repo,
    IPublishEndpoint publisher)
    : IStep<CreateApiAccessRuleDelegate, CreateApiAccessRuleContext, IMaybe<ApiAccessRuleDto>>
{
    public async Task<IMaybe<ApiAccessRuleDto>> InvokeAsync(
        CreateApiAccessRuleContext context,
        CreateApiAccessRuleDelegate next,
        CancellationToken token = default)
    {
        var entity = context.Request.ToEntity();

        var stored = (ApiAccessRuleEntity)await repo.AddOrUpdateAsync(entity, token: token);
        await publisher.Publish(new ApiAccessRestrictionsChanged(DateTimeOffset.UtcNow), token);

        return Maybe.Create(stored.ToDto());
    }
}
