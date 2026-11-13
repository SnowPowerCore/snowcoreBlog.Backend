using MinimalStepifiedSystem.Base;
using snowcoreBlog.Backend.Core.Entities.ApiAccessRestrictions;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Context;

public class DeleteApiAccessRuleContext(Guid id) : BaseGenericContext
{
    public Guid Id { get; } = id;

    public ApiAccessRuleEntity? Rule { get; set; }
}
