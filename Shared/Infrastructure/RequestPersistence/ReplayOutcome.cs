using snowcoreBlog.Backend.Shared.Core.Entities.RequestPersistence;

namespace snowcoreBlog.Backend.Infrastructure.RequestPersistence;

public sealed record ReplayOutcome(
    RequestPersistenceStatus Status,
    int? StatusCode,
    string? ResponsePayload,
    string? Note)
{
    public static ReplayOutcome AuthFailed { get; } =
        new(RequestPersistenceStatus.Failed, null, null, "Replay re-authentication failed; manual handling required.");

    public static ReplayOutcome AntiforgeryUnavailable { get; } =
        new(RequestPersistenceStatus.Failed, null, null, "Antiforgery token endpoint did not produce a usable token; manual handling required.");

    public static ReplayOutcome NoSelfAddress { get; } =
        new(RequestPersistenceStatus.RetryNeeded, null, null, "Self base address not resolvable yet.");
}
