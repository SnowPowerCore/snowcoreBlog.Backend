using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Context;

public class CreateApiAccessRuleContext(CreateApiAccessRuleDto request) : BaseGenericContext
{
    public CreateApiAccessRuleDto Request { get; } = request;
}
