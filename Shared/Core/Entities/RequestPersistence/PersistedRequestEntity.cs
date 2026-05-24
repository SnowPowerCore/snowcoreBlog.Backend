using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core.Entities.RequestPersistence;

public record PersistedRequestEntity : BaseEntity
{
    // Logical request identifier (mirrors Id but kept as explicit field)
    public Guid RequestId { get; init; }

    public string? IdempotencyKey { get; init; }

    public RequestPersistenceStatus Status { get; init; } = RequestPersistenceStatus.Received;

    public string RequestPayload { get; init; } = string.Empty;

    public string? ResponsePayload { get; init; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; init; } = DateTimeOffset.UtcNow;

    public int RetryCount { get; init; }
}