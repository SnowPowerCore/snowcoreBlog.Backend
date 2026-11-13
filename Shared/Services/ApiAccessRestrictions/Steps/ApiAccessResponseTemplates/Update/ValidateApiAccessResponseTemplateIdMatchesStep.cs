using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessResponseTemplates.Update;

public sealed class ValidateApiAccessResponseTemplateIdMatchesStep
    : IStep<UpdateApiAccessResponseTemplateDelegate, UpdateApiAccessResponseTemplateContext, IMaybe<ApiAccessResponseTemplateDto>>
{
    public Task<IMaybe<ApiAccessResponseTemplateDto>> InvokeAsync(
        UpdateApiAccessResponseTemplateContext context,
        UpdateApiAccessResponseTemplateDelegate next,
        CancellationToken token = default)
    {
        if (context.Request.Id != context.RouteId)
        {
            return Task.FromResult(ApiAccessResponseTemplateIdMismatchError<ApiAccessResponseTemplateDto>.Create(
                "Template id in the request body does not match the route id."));
        }

        return next(context, token);
    }
}
