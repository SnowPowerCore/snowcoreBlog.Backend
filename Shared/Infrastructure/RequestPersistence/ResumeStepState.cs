namespace snowcoreBlog.Backend.Infrastructure.RequestPersistence;

/// <summary>One completed step of an interrupted pipeline (from persisted checkpoints).</summary>
public sealed record ResumeStepState(
    int StepIndex,
    string StepTypeName,
    string SnapshotType,
    string SnapshotJson);
