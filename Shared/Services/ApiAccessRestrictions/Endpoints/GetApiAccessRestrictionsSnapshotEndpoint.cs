using FastEndpoints;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Core.Entities.ApiAccessRestrictions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints;

public class GetApiAccessRestrictionsSnapshotEndpoint(IIpRestrictionRepository ipRepo,
                                                      IRegionRestrictionRepository regionRepo,
                                                      IApiAccessRuleRepository ruleRepo,
                                                      IApiAccessResponseTemplateRepository templateRepo) : EndpointWithoutRequest<ApiAccessRestrictionsSnapshotDto>
{
    public override void Configure()
    {
        Get("snapshot");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var rules = await ruleRepo.GetAllAsync(ct);
        var templates = await templateRepo.GetAllAsync(ct);
        var ipRestrictions = await ipRepo.GetAllAsync(ct);
        var regionRestrictions = await regionRepo.GetAllAsync(ct);

        var snapshot = new ApiAccessRestrictionsSnapshotDto
        {
            GeneratedAt = DateTimeOffset.UtcNow,
            Rules = rules.Select(static r => new ApiAccessRuleSnapshotDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Enabled = r.Enabled,
                Priority = r.Priority,
                Action = (ApiAccessRestrictionActionDto)r.Action,
                CreatedAt = r.CreatedAt,
                ExpiresAt = r.ExpiresAt,
                Methods = r.Methods,
                PathPatterns = r.PathPatterns,
                Tags = r.Tags,
                IpRanges = r.IpRanges,
                RegionCodes = r.RegionCodes,
                ResponseTemplateId = r.ResponseTemplateId
            }).ToList(),
            ResponseTemplates = templates.Select(static t => new ApiAccessResponseTemplateSnapshotDto
            {
                Id = t.Id,
                Name = t.Name,
                StatusCode = t.StatusCode,
                ContentType = t.ContentType,
                BodyJson = t.BodyJson,
                Reason = t.Reason,
                CreatedAt = t.CreatedAt
            }).ToList(),
            IpRestrictions = ipRestrictions.Select(static i => new IpRestrictionSnapshotDto
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                IsBlocked = i.IsBlocked,
                CreatedAt = i.CreatedAt,
                ExpiresAt = i.ExpiresAt,
                IpRanges = i.IpRanges
            }).ToList(),
            RegionRestrictions = regionRestrictions.Select(static r => new RegionRestrictionSnapshotDto
            {
                Id = r.Id,
                RegionCode = r.RegionCode,
                Level = r.Level switch
                {
                    RestrictionLevel.RequireCaptcha => ApiAccessRestrictionActionDto.RequireCaptcha,
                    RestrictionLevel.AllowWithWarning => ApiAccessRestrictionActionDto.AllowWithWarning,
                    _ => ApiAccessRestrictionActionDto.Block
                },
                CreatedAt = r.CreatedAt,
                ExpiresAt = r.ExpiresAt,
                AffectedPaths = r.AffectedPaths
            }).ToList()
        };

        await Send.ResponseAsync(snapshot, cancellation: ct);
    }
}