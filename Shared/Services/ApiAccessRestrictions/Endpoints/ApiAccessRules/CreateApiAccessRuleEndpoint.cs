using FastSharp.Modules.Core;
using MaybeResults;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessRules.Create;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessRules;

public sealed class CreateApiAccessRuleEndpoint : IEndpoint
{
    [StepifiedProcess(Steps = [
        typeof(CreateApiAccessRuleStep),
    ])]
    public CreateApiAccessRuleDelegate CreateApiAccessRule { get; } = default!;

    public void Map(RouteGroupBuilder app)
    {
        app.MapPost("/v1", async (CreateApiAccessRuleDto request, CancellationToken ct) =>
        {
            var result = await CreateApiAccessRule(new CreateApiAccessRuleContext(request), ct);

            return result.ToEnvelopeResult();
        });
    }
}
