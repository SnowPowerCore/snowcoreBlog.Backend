using FastSharp.Modules.Core;
using MaybeResults;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessResponseTemplates.Update;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessResponseTemplates;

public sealed class UpdateApiAccessResponseTemplateEndpoint : IEndpoint
{
    [StepifiedProcess(Steps = [
        typeof(ValidateApiAccessResponseTemplateIdMatchesStep),
        typeof(ResolveApiAccessResponseTemplateForUpdateStep),
        typeof(UpdateApiAccessResponseTemplateStep),
    ])]
    public UpdateApiAccessResponseTemplateDelegate UpdateApiAccessResponseTemplate { get; } = default!;

    public void Map(RouteGroupBuilder app)
    {
        app.MapPut("/{id:guid}/v1", async (Guid id, UpdateApiAccessResponseTemplateDto request, CancellationToken ct) =>
        {
            var context = new UpdateApiAccessResponseTemplateContext(request, id);
            var result = await UpdateApiAccessResponseTemplate(context, ct);

            return result.ToEnvelopeResult();
        });
    }
}
