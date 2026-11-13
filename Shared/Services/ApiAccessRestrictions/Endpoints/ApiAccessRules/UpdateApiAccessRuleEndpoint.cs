using FastSharp.Modules.Core;
using MaybeResults;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessRules.Update;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessRules;

public sealed class UpdateApiAccessRuleEndpoint : IEndpoint
{
    [StepifiedProcess(Steps = [
        typeof(ValidateApiAccessRuleIdMatchesStep),
        typeof(ResolveApiAccessRuleForUpdateStep),
        typeof(UpdateApiAccessRuleStep),
    ])]
    public UpdateApiAccessRuleDelegate UpdateApiAccessRule { get; } = default!;

    public void Map(RouteGroupBuilder app)
    {
        app.MapPut("/{id:guid}/v1", async (Guid id, UpdateApiAccessRuleDto request, CancellationToken ct) =>
        {
            var context = new UpdateApiAccessRuleContext(request, id);
            var result = await UpdateApiAccessRule(context, ct);

            return result.ToEnvelopeResult();
        });
    }
}
