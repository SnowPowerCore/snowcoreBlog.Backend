using FastEndpoints;
using MassTransit;
using snowcoreBlog.Backend.ApiAccessRestrictions.Constants;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;
using snowcoreBlog.Backend.Core.Entities.ApiAccessRestrictions;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessResponseTemplates;

public class DeleteApiAccessResponseTemplateEndpoint(IApiAccessResponseTemplateRepository repo,
                                                     IPublishEndpoint publisher) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("templates/{id:guid}");
        Claims(AdminClaimConstants.AdminAccountClaimKey);
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var entity = await repo.GetByIdAsync(id, ct);
        if (entity is default(ApiAccessResponseTemplateEntity))
        {
            await Send.NotFoundAsync(cancellation: ct);
            return;
        }
        await repo.RemoveAsync(entity, token: ct);
        await publisher.Publish(new ApiAccessRestrictionsChanged(DateTimeOffset.UtcNow), ct);
        await Send.OkAsync(cancellation: ct);
    }
}