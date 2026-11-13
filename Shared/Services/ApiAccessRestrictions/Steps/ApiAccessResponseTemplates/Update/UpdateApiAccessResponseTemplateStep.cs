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

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessResponseTemplates.Update;

public sealed class UpdateApiAccessResponseTemplateStep(
    IApiAccessResponseTemplateRepository repo,
    IPublishEndpoint publisher)
    : IStep<UpdateApiAccessResponseTemplateDelegate, UpdateApiAccessResponseTemplateContext, IMaybe<ApiAccessResponseTemplateDto>>
{
    public async Task<IMaybe<ApiAccessResponseTemplateDto>> InvokeAsync(
        UpdateApiAccessResponseTemplateContext context,
        UpdateApiAccessResponseTemplateDelegate next,
        CancellationToken token = default)
    {
        context.Request.ApplyTo(context.Template!);

        var stored = (ApiAccessResponseTemplateEntity)await repo.AddOrUpdateAsync(context.Template!, context.Template!.Id, token: token);
        await publisher.Publish(new ApiAccessRestrictionsChanged(DateTimeOffset.UtcNow), token);

        return Maybe.Create(stored.ToDto());
    }
}
