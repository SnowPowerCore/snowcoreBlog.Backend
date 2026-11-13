using FastSharp.Modules.Core;
using MaybeResults;
using snowcoreBlog.Backend.ApiAccessRestrictions.Extensions;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessRules;

/// <summary>
/// Lists all rules. Short and stable by design, so it consumes the repository
/// directly (non-stepified).
/// </summary>
public sealed class GetApiAccessRulesEndpoint(IApiAccessRuleRepository ruleRepository) : IEndpoint
{
    public void Map(RouteGroupBuilder app)
    {
        app.MapGet("/v1", async (CancellationToken ct) =>
        {
            var rules = await ruleRepository.GetAllAsync(ct);

            var result = Maybe.Create<IReadOnlyList<ApiAccessRuleDto>>(
                rules.Select(static r => r.ToDto()).ToList());

            return result.ToEnvelopeResult();
        });
    }
}
