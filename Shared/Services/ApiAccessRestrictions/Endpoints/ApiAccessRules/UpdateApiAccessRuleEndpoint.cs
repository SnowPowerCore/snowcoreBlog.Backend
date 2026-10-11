using FastEndpoints;
using MassTransit;
using snowcoreBlog.Backend.ApiAccessRestrictions.Repositories.Marten;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;
using snowcoreBlog.Backend.Core.Entities.ApiAccessRestrictions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessRules;

public class UpdateApiAccessRuleEndpoint(IApiAccessRuleRepository repo,
                                         IPublishEndpoint publisher) : Endpoint<UpdateApiAccessRuleDto>
{
    public override void Configure()
    {
        Put("rules/{id:guid}");
        AllowAnonymous(); // TODO: restrict to admin
        Version(1);
    }

    public override async Task HandleAsync(UpdateApiAccessRuleDto req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        if (req.Id != id)
        {
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var existing = await repo.GetByIdAsync(id, ct);
        if (existing is default(ApiAccessRuleEntity))
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        existing.Name = req.Name;
        existing.Description = req.Description;
        existing.Enabled = req.Enabled;
        existing.Priority = req.Priority;
        existing.Action = (RestrictionLevel)req.Action;
        existing.Methods = req.Methods?.ToList() ?? [];
        existing.PathPatterns = req.PathPatterns?.ToList() ?? [];
        existing.Tags = req.Tags?.ToList() ?? [];
        existing.IpRanges = req.IpRanges?.ToList() ?? [];
        existing.RegionCodes = req.RegionCodes?.ToList() ?? [];
        existing.ResponseTemplateId = req.ResponseTemplateId;
        existing.ExpiresAt = req.ExpiresAt;

        await repo.SaveAsync(existing, ct);
        await publisher.Publish(new ApiAccessRestrictionsChanged(DateTimeOffset.UtcNow), ct);
        await Send.OkAsync(cancellation: ct);
    }
}