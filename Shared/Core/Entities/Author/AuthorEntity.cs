using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core.Entities.Author;

public record AuthorEntity : BaseEntity
{
    public required Guid UserId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
}
