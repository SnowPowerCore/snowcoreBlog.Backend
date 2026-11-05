using FastEndpoints;
using MassTransit;
using snowcoreBlog.Backend.ApiAccessRestrictions.Constants;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Repositories.Marten;
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
        Claims(AdminClaimConstants.AdminAccountClaimKey);
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

        existing = existing with
        {
            Name = req.Name,
            Description = req.Description,
            Enabled = req.Enabled,
            Priority = req.Priority,
            Action = (RestrictionLevel)req.Action,
            Methods = req.Methods?.ToList(),
            PathPatterns = req.PathPatterns?.ToList(),
            Tags = req.Tags?.ToList(),
            IpRanges = req.IpRanges?.ToList(),
            RegionCodes = req.RegionCodes?.ToList(),
            ResponseTemplateId = req.ResponseTemplateId,
            ExpiresAt = req.ExpiresAt
        };

        await repo.AddOrUpdateAsync(existing, existing.Id, token: ct);
        await publisher.Publish(new ApiAccessRestrictionsChanged(DateTimeOffset.UtcNow), ct);
        await Send.OkAsync(cancellation: ct);
    }
}