using FastEndpoints;
using snowcoreBlog.Backend.ApiAccessRestrictions.Entities;
using snowcoreBlog.Backend.ApiAccessRestrictions.Repositories.Marten;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessRules;

public class GetApiAccessRulesEndpoint(IApiAccessRuleRepository repo) : EndpointWithoutRequest<IReadOnlyList<ApiAccessRuleEntity>>
{
    public override void Configure()
    {
        Get("rules");
        AllowAnonymous(); // TODO: restrict to admin
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var all = await repo.GetAllAsync(ct);
        await Send.ResponseAsync(all, cancellation: ct);
    }
}