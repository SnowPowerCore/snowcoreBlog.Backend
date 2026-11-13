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

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessResponseTemplates.Delete;

public sealed class DeleteApiAccessResponseTemplateStep(
    IApiAccessResponseTemplateRepository repo,
    IPublishEndpoint publisher)
    : IStep<DeleteApiAccessResponseTemplateDelegate, DeleteApiAccessResponseTemplateContext, IMaybe<ApiAccessResponseTemplateDto>>
{
    public async Task<IMaybe<ApiAccessResponseTemplateDto>> InvokeAsync(
        DeleteApiAccessResponseTemplateContext context,
        DeleteApiAccessResponseTemplateDelegate next,
        CancellationToken token = default)
    {
        await repo.RemoveAsync(context.Template!, token: token);
        await publisher.Publish(new ApiAccessRestrictionsChanged(DateTimeOffset.UtcNow), token);

        return Maybe.Create(context.Template!.ToDto());
    }
}
