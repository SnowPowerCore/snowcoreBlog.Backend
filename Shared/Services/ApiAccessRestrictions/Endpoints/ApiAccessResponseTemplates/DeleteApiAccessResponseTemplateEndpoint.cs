using FastSharp.Modules.Core;
using MaybeResults;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessResponseTemplates.Delete;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessResponseTemplates;

public sealed class DeleteApiAccessResponseTemplateEndpoint : IEndpoint
{
    [StepifiedProcess(Steps = [
        typeof(ResolveApiAccessResponseTemplateForDeleteStep),
        typeof(DeleteApiAccessResponseTemplateStep),
    ])]
    public DeleteApiAccessResponseTemplateDelegate DeleteApiAccessResponseTemplate { get; } = default!;

    public void Map(RouteGroupBuilder app)
    {
        app.MapDelete("/{id:guid}/v1", async (Guid id, CancellationToken ct) =>
        {
            var context = new DeleteApiAccessResponseTemplateContext(id);
            var result = await DeleteApiAccessResponseTemplate(context, ct);

            return result.ToEnvelopeResult();
        });
    }
}
