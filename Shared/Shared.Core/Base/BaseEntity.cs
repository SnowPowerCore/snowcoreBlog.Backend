namespace snowcoreBlog.Backend.Shared.Core.Base;

public abstract record BaseEntity()
{
    public Guid Id { get; init; }
}