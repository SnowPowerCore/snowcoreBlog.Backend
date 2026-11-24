using MinimalStepifiedSystem.Base;
using snowcoreBlog.Backend.ApiAccessRestrictions.Core.Entities.ApiAccessRestrictions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Context;

public class UpdateApiAccessResponseTemplateContext(UpdateApiAccessResponseTemplateDto request, Guid routeId) : BaseGenericContext
{
    public UpdateApiAccessResponseTemplateDto Request { get; } = request;

    public Guid RouteId { get; } = routeId;

    public ApiAccessResponseTemplateEntity? Template { get; set; }
}
