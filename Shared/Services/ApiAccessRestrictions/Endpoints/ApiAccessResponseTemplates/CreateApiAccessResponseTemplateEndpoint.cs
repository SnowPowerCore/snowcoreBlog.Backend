using FastSharp.Modules.Core;
using MaybeResults;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessResponseTemplates.Create;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessResponseTemplates;

public sealed class CreateApiAccessResponseTemplateEndpoint : IEndpoint
{
    [StepifiedProcess(Steps = [
        typeof(CreateApiAccessResponseTemplateStep),
    ])]
    public CreateApiAccessResponseTemplateDelegate CreateApiAccessResponseTemplate { get; } = default!;

    public void Map(RouteGroupBuilder app)
    {
        app.MapPost("/v1", async (CreateApiAccessResponseTemplateDto request, CancellationToken ct) =>
        {
            var result = await CreateApiAccessResponseTemplate(new CreateApiAccessResponseTemplateContext(request), ct);

            return result.ToEnvelopeResult();
        });
    }
}
