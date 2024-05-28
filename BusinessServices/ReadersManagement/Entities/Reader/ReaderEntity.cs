using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core;

public record ReaderEntity : BaseEntity
{
    public required Guid UserId { get; init; }

    public string NickName { get; init; } = string.Empty;
}