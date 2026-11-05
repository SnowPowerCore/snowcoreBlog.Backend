using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core.Entities.RequestPersistence;

/// <summary>
/// Durable claim for a persist-tagged request. Written (in its own session/transaction) before any
/// business logic runs, so an interrupted execution always leaves a replayable trace behind.
/// <para>
/// Lifecycle: Persisted → Processing (lease) → Succeeded | Failed | RetryNeeded. First-attempt
/// terminal finishes are purged to a tombstone stub (payload fields emptied) while recovered
/// requests keep their full payload for audit.
/// </para>
/// </summary>
public record PersistedRequestEntity : BaseEntity
{
    /// <summary>Mirrors <see cref="BaseEntity.Id"/> for query convenience.</summary>
    public Guid RequestId { get; init; }

    /// <summary>Client-supplied dedup key; unique across the service database.</summary>
    public string IdempotencyKey { get; init; } = string.Empty;

    /// <summary>SHA-256 (base64url) of the request body — detects key reuse with a different payload.</summary>
    public string RequestHash { get; init; } = string.Empty;

    public RequestPersistenceStatus Status { get; init; } = RequestPersistenceStatus.Persisted;

    public string Method { get; init; } = string.Empty;

    public string Path { get; init; } = string.Empty;

    public string QueryString { get; init; } = string.Empty;

    public string ContentType { get; init; } = string.Empty;

    /// <summary>Auth-relevant header snapshot (cookies/authorization) for replay re-authentication.</summary>
    public string? HeadersJson { get; init; }

    /// <summary>True when the endpoint streams (SSE) — response is not captured, only completion status.</summary>
    public bool IsStreamingResponse { get; init; }

    public string RequestPayload { get; init; } = string.Empty;

    public string? ResponsePayload { get; init; }

    public int? ResponseStatusCode { get; init; }

    /// <summary>Execution lease held by the running instance; stale leases are taken over by the recovery worker.</summary>
    public DateTimeOffset? LeaseUntil { get; init; }

    /// <summary>
    /// Ownership token of the current lease — set per takeover, verified on re-read. Timestamp-only
    /// verification is unreliable (Postgres timestamptz truncates sub-microsecond precision, and
    /// concurrent takers can compute identical timestamps); a GUID makes the win-check unambiguous.
    /// </summary>
    public Guid? LeaseId { get; init; }

    /// <summary>Earliest time the recovery worker may retry (exponential backoff).</summary>
    public DateTimeOffset? NextAttemptAt { get; init; }

    public int RetryCount { get; init; }

    /// <summary>Operator-facing note (last error, replay auth failure reason, ...).</summary>
    public string? Note { get; init; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; init; } = DateTimeOffset.UtcNow;
}
