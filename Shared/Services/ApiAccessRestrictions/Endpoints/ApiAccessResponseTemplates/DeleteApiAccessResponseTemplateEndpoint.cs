using FastEndpoints;
using snowcoreBlog.Backend.ApiAccessRestrictions.Repositories.Marten;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessResponseTemplates;

public class DeleteApiAccessResponseTemplateEndpoint(IApiAccessResponseTemplateRepository repo) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("templates/{id:guid}");
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