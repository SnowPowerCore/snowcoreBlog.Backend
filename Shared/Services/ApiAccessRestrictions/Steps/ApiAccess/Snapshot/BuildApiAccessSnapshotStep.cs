using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.Backend.ApiAccessRestrictions.Core.Entities.ApiAccessRestrictions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccess.Snapshot;

public sealed class BuildApiAccessSnapshotStep
    : IStep<GetApiAccessRestrictionsSnapshotDelegate, GetApiAccessRestrictionsSnapshotContext, IMaybe<ApiAccessRestrictionsSnapshotDto>>
{
    public Task<IMaybe<ApiAccessRestrictionsSnapshotDto>> InvokeAsync(
        GetApiAccessRestrictionsSnapshotContext context,
        GetApiAccessRestrictionsSnapshotDelegate next,
        CancellationToken token = default)
    {
        var snapshot = new ApiAccessRestrictionsSnapshotDto
        {
            GeneratedAt = DateTimeOffset.UtcNow,
            Rules = context.Rules.Select(static r => new ApiAccessRuleSnapshotDto
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
            ResponseTemplates = context.Templates.Select(static t => new ApiAccessResponseTemplateSnapshotDto
            {
                Id = t.Id,
                Name = t.Name,
                StatusCode = t.StatusCode,
                ContentType = t.ContentType,
                BodyJson = t.BodyJson,
                Reason = t.Reason,
                CreatedAt = t.CreatedAt
            }).ToList(),
            IpRestrictions = context.IpRestrictions.Select(static i => new IpRestrictionSnapshotDto
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                IsBlocked = i.IsBlocked,
                CreatedAt = i.CreatedAt,
                ExpiresAt = i.ExpiresAt,
                IpRanges = i.IpRanges
            }).ToList(),
            RegionRestrictions = context.RegionRestrictions.Select(static r => new RegionRestrictionSnapshotDto
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

        return Task.FromResult(Maybe.Create(snapshot));
    }
}
