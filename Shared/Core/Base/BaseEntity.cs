namespace snowcoreBlog.Backend.Core.Base;

public abstract record BaseEntity()
{
    public required Guid Id { get; init; }
}