using MinimalStepifiedSystem.Base;
using snowcoreBlog.Backend.Core.Entities.ApiAccessRestrictions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Context;

public class GetApiAccessRestrictionsSnapshotContext : BaseGenericContext
{
    public IEnumerable<ApiAccessRuleEntity> Rules { get; set; } = [];

    public IEnumerable<ApiAccessResponseTemplateEntity> Templates { get; set; } = [];

    public IEnumerable<IpRestrictionEntity> IpRestrictions { get; set; } = [];

    public IEnumerable<RegionRestrictionEntity> RegionRestrictions { get; set; } = [];
}
