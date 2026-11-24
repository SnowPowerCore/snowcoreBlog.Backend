using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.ApiAccessRestrictions.Core.Entities.ApiAccessRestrictions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessResponseTemplates.Update;

public sealed class ResolveApiAccessResponseTemplateForUpdateStep(IApiAccessResponseTemplateRepository repo)
    : IStep<UpdateApiAccessResponseTemplateDelegate, UpdateApiAccessResponseTemplateContext, IMaybe<ApiAccessResponseTemplateDto>>
{
    public async Task<IMaybe<ApiAccessResponseTemplateDto>> InvokeAsync(
        UpdateApiAccessResponseTemplateContext context,
        UpdateApiAccessResponseTemplateDelegate next,
        CancellationToken token = default)
    {
        var existing = await repo.GetByIdAsync(context.RouteId, token);
        if (existing is default(ApiAccessResponseTemplateEntity))
        {
            return ApiAccessResponseTemplateNotFoundError<ApiAccessResponseTemplateDto>.Create(
                $"Api access response template with ID '{context.RouteId}' was not found.");
        }

        context.Template = existing;
        return await next(context, token);
    }
}
