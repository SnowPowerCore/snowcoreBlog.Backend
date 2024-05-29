namespace snowcoreBlog.Backend.Core.Base;

public abstract record BaseEntity()
{
    public Guid Id { get; init; }
}