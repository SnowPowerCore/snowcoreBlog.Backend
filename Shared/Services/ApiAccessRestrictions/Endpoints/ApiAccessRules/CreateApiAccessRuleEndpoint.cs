using FastEndpoints;
using MassTransit;
using snowcoreBlog.Backend.ApiAccessRestrictions.Repositories.Marten;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;
using snowcoreBlog.Backend.Core.Entities.ApiAccessRestrictions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessRules;

public class CreateApiAccessRuleEndpoint(IApiAccessRuleRepository repo,
                                         IPublishEndpoint publisher) : Endpoint<CreateApiAccessRuleDto, ApiAccessRuleEntity>
{
    public override void Configure()
    {
        Post("rules");
        AllowAnonymous(); // TODO: restrict to admin
        Version(1);
    }

    public override async Task HandleAsync(CreateApiAccessRuleDto req, CancellationToken ct)
    {
        var entity = new ApiAccessRuleEntity
        {
            Name = req.Name,
            Description = req.Description,
            Enabled = req.Enabled,
            Priority = req.Priority,
            Action = (RestrictionLevel)req.Action,
            Methods = req.Methods?.ToList() ?? [],
            PathPatterns = req.PathPatterns?.ToList() ?? [],
            Tags = req.Tags?.ToList() ?? [],
            IpRanges = req.IpRanges?.ToList() ?? [],
            RegionCodes = req.RegionCodes?.ToList() ?? [],
            ResponseTemplateId = req.ResponseTemplateId,
            ExpiresAt = req.ExpiresAt
        };

        await repo.SaveAsync(entity, ct);
        await publisher.Publish(new ApiAccessRestrictionsChanged(DateTimeOffset.UtcNow), ct);
        await Send.CreatedAtAsync<GetApiAccessRulesEndpoint>(entity, cancellation: ct);
    }
}