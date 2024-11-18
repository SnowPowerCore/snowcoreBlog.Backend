using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core.Entities.Reader;

public record ReaderEntity : BaseEntity
{
    public required Guid UserId { get; init; }

    public string NickName { get; init; } = string.Empty;
}