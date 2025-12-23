using FastEndpoints;
using snowcoreBlog.Backend.ApiAccessRestrictions.Repositories.Marten;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessRules;

public class DeleteApiAccessRuleEndpoint(IApiAccessRuleRepository repo) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("rules/{id:guid}");
        AllowAnonymous(); // TODO: restrict to admin
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        await repo.DeleteAsync(id, ct);
        await Send.OkAsync(cancellation: ct);
    }
}