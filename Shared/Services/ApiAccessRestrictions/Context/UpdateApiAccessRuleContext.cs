using MinimalStepifiedSystem.Base;
using snowcoreBlog.Backend.Core.Entities.ApiAccessRestrictions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Context;

public class UpdateApiAccessRuleContext(UpdateApiAccessRuleDto request, Guid routeId) : BaseGenericContext
{
    public UpdateApiAccessRuleDto Request { get; } = request;

    public Guid RouteId { get; } = routeId;

    public ApiAccessRuleEntity? Rule { get; set; }
}
