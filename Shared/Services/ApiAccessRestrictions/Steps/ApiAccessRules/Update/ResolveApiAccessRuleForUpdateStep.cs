using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.ApiAccessRestrictions.Core.Entities.ApiAccessRestrictions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessRules.Update;

public sealed class ResolveApiAccessRuleForUpdateStep(IApiAccessRuleRepository repo)
    : IStep<UpdateApiAccessRuleDelegate, UpdateApiAccessRuleContext, IMaybe<ApiAccessRuleDto>>
{
    public async Task<IMaybe<ApiAccessRuleDto>> InvokeAsync(
        UpdateApiAccessRuleContext context,
        UpdateApiAccessRuleDelegate next,
        CancellationToken token = default)
    {
        var existing = await repo.GetByIdAsync(context.RouteId, token);
        if (existing is default(ApiAccessRuleEntity))
        {
            return ApiAccessRuleNotFoundError<ApiAccessRuleDto>.Create(
                $"Api access rule with ID '{context.RouteId}' was not found.");
        }

        context.Rule = existing;
        return await next(context, token);
    }
}
