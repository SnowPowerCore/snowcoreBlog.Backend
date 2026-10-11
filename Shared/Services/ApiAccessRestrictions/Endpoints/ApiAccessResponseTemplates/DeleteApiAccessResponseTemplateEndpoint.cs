using FastEndpoints;
using MassTransit;
using snowcoreBlog.Backend.ApiAccessRestrictions.Repositories.Marten;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessResponseTemplates;

public class DeleteApiAccessResponseTemplateEndpoint(IApiAccessResponseTemplateRepository repo,
                                                     IPublishEndpoint publisher) : EndpointWithoutRequest
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
        await publisher.Publish(new ApiAccessRestrictionsChanged(DateTimeOffset.UtcNow), ct);
        await Send.OkAsync(cancellation: ct);
    }
}