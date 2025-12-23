using FastEndpoints;
using snowcoreBlog.Backend.ApiAccessRestrictions.Entities;
using snowcoreBlog.Backend.ApiAccessRestrictions.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessRules;

public class CreateApiAccessRuleEndpoint(IApiAccessRuleRepository repo) : Endpoint<CreateApiAccessRuleDto, ApiAccessRuleEntity>
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
            Action = req.Action,
            Methods = req.Methods?.ToList() ?? [],
            PathPatterns = req.PathPatterns?.ToList() ?? [],
            Tags = req.Tags?.ToList() ?? [],
            IpRanges = req.IpRanges?.ToList() ?? [],
            RegionCodes = req.RegionCodes?.ToList() ?? [],
            ResponseTemplateId = req.ResponseTemplateId,
            ExpiresAt = req.ExpiresAt
        };

        await repo.SaveAsync(entity, ct);
        await Send.CreatedAtAsync<GetApiAccessRulesEndpoint>(entity, cancellation: ct);
    }
}