using MinimalStepifiedSystem.Base;
using snowcoreBlog.Backend.Core.Entities.ApiAccessRestrictions;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Context;

public class DeleteApiAccessResponseTemplateContext(Guid id) : BaseGenericContext
{
    public Guid Id { get; } = id;

    public ApiAccessResponseTemplateEntity? Template { get; set; }
}
