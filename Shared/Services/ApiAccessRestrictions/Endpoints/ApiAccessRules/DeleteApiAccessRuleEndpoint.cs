using FastSharp.Modules.Core;
using MaybeResults;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessRules.Delete;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessRules;

public sealed class DeleteApiAccessRuleEndpoint : IEndpoint
{
    [StepifiedProcess(Steps = [
        typeof(ResolveApiAccessRuleForDeleteStep),
        typeof(DeleteApiAccessRuleStep),
    ])]
    public DeleteApiAccessRuleDelegate DeleteApiAccessRule { get; } = default!;

    public void Map(RouteGroupBuilder app)
    {
        app.MapDelete("/{id:guid}/v1", async (Guid id, CancellationToken ct) =>
        {
            var context = new DeleteApiAccessRuleContext(id);
            var result = await DeleteApiAccessRule(context, ct);

            return result.ToEnvelopeResult();
        });
    }
}
