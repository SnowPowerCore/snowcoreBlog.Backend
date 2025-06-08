using MinimalStepifiedSystem.Base;

namespace snowcoreBlog.Backend.AuthorsManagement.Context;

public class BecomeAuthorAccountContext : BaseGenericContext
{
    public BecomeAuthorAccountContext(Guid userId, string displayName)
    {
        UserId = userId;
        DisplayName = displayName;
    }

    public Guid UserId { get; }
    public string DisplayName { get; }
}
