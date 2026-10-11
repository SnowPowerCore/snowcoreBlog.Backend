using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;

public sealed class ApiAccessRestrictionsSnapshotStore : IApiAccessRestrictionsSnapshotStore
{
    // Single volatile reference so the DTO and its compiled form are always swapped
    // atomically - a reader can never observe a new DTO paired with a stale compiled
    // snapshot (which the previous two-field implementation allowed briefly).
    private volatile SnapshotState? _state;

    public ApiAccessRestrictionsSnapshotDto? Current => _state?.Dto;

    public CompiledApiAccessRestrictionsSnapshot? CurrentCompiled => _state?.Compiled;

    public void Update(ApiAccessRestrictionsSnapshotDto snapshot) =>
        _state = new SnapshotState(snapshot, CompiledApiAccessRestrictionsSnapshot.Compile(snapshot));

    private sealed record SnapshotState(ApiAccessRestrictionsSnapshotDto Dto, CompiledApiAccessRestrictionsSnapshot Compiled);
}