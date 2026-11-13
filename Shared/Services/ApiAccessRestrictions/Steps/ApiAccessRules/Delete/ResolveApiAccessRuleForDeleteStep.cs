using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Core.Entities.ApiAccessRestrictions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessRules.Delete;

public sealed class ResolveApiAccessRuleForDeleteStep(IApiAccessRuleRepository repo)
    : IStep<DeleteApiAccessRuleDelegate, DeleteApiAccessRuleContext, IMaybe<ApiAccessRuleDto>>
{
    public async Task<IMaybe<ApiAccessRuleDto>> InvokeAsync(
        DeleteApiAccessRuleContext context,
        DeleteApiAccessRuleDelegate next,
        CancellationToken token = default)
    {
        var existing = await repo.GetByIdAsync(context.Id, token);
        if (existing is default(ApiAccessRuleEntity))
        {
            return ApiAccessRuleNotFoundError<ApiAccessRuleDto>.Create(
                $"Api access rule with ID '{context.Id}' was not found.");
        }

        context.Rule = existing;
        return await next(context, token);
    }
}
