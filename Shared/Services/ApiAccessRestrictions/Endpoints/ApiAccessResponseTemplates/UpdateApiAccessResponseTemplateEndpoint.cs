using FastEndpoints;
using MassTransit;
using snowcoreBlog.Backend.ApiAccessRestrictions.Constants;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;
using snowcoreBlog.Backend.Core.Entities.ApiAccessRestrictions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessResponseTemplates;

public class UpdateApiAccessResponseTemplateEndpoint(IApiAccessResponseTemplateRepository repo,
                                                     IPublishEndpoint publisher) : Endpoint<UpdateApiAccessResponseTemplateDto>
{
    public override void Configure()
    {
        Put("templates/{id:guid}");
        Claims(AdminClaimConstants.AdminAccountClaimKey);
        Version(1);
    }

    public override async Task HandleAsync(UpdateApiAccessResponseTemplateDto req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        if (req.Id != id)
        {
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var existing = await repo.GetByIdAsync(id, ct);
        if (existing is default(ApiAccessResponseTemplateEntity))
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        existing = existing with
        {
            Name = req.Name,
            StatusCode = req.StatusCode,
            ContentType = req.ContentType,
            BodyJson = req.BodyJson,
            Reason = req.Reason
        };

        await repo.AddOrUpdateAsync(existing, existing.Id, token: ct);
        await publisher.Publish(new ApiAccessRestrictionsChanged(DateTimeOffset.UtcNow), ct);
        await Send.OkAsync(cancellation: ct);
    }
}