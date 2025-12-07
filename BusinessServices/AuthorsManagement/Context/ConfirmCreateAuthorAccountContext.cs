using MinimalStepifiedSystem.Base;

namespace snowcoreBlog.Backend.AuthorsManagement.Context;

public class BecomeAuthorAccountContext(Guid userId, string displayName) : BaseGenericContext
{
    public Guid UserId { get; } = userId;
    public string DisplayName { get; } = displayName;
}
