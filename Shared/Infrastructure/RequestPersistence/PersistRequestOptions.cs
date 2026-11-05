namespace snowcoreBlog.Backend.Infrastructure.RequestPersistence;

public sealed class PersistRequestOptions
{
    /// <summary>Execution lease duration; stale leases are taken over by the recovery worker.</summary>
    public TimeSpan LeaseTtl { get; set; } = TimeSpan.FromMinutes(10);

    /// <summary>Recovery worker polling interval.</summary>
    public TimeSpan RecoveryPollInterval { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>Claims stuck in Persisted (never executed) older than this are replayed from scratch.</summary>
    public TimeSpan RecoverPersistedAfter { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>Automatic replay attempts before a row is marked Failed (manual handling).</summary>
    public int RetryBudget { get; set; } = 3;

    /// <summary>Exponential backoff base (2^retryCount multiplier) for RetryNeeded replays.</summary>
    public TimeSpan RetryBackoffBase { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>Recovery worker batch size per poll.</summary>
    public int RecoveryBatchSize { get; set; } = 10;

    /// <summary>Path of the antiforgery token endpoint used to mint fresh tokens for replays.</summary>
    public string AntiforgeryTokenPath { get; set; } = "/v1/antiforgerytoken";

    /// <summary>Purge plain first-attempt finishes to tombstone stubs (dedup identity preserved).</summary>
    public bool TombstonePlainFinishes { get; set; } = true;

    /// <summary>Keep full records for requests that went through recovery/replay (audit trail).</summary>
    public bool KeepRecoveredForAudit { get; set; } = true;

    public DateTimeOffset ComputeBackoff(int retryCount, DateTimeOffset now)
    {
        var exponent = Math.Clamp(retryCount, 0, 5);
        return now + (RetryBackoffBase * Math.Pow(2, exponent));
    }
}
