using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessRules.Update;

public sealed class ValidateApiAccessRuleIdMatchesStep
    : IStep<UpdateApiAccessRuleDelegate, UpdateApiAccessRuleContext, IMaybe<ApiAccessRuleDto>>
{
    public Task<IMaybe<ApiAccessRuleDto>> InvokeAsync(
        UpdateApiAccessRuleContext context,
        UpdateApiAccessRuleDelegate next,
        CancellationToken token = default)
    {
        if (context.Request.Id != context.RouteId)
        {
            return Task.FromResult(ApiAccessRuleIdMismatchError<ApiAccessRuleDto>.Create(
                "Rule id in the request body does not match the route id."));
        }

        return next(context, token);
    }
}
