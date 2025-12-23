using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;

public sealed class ApiAccessRestrictionsSnapshotStore : IApiAccessRestrictionsSnapshotStore
{
    private volatile ApiAccessRestrictionsSnapshotDto? _current;
    private volatile CompiledApiAccessRestrictionsSnapshot? _compiled;

    public ApiAccessRestrictionsSnapshotDto? Current => _current;

    public CompiledApiAccessRestrictionsSnapshot? CurrentCompiled => _compiled;

    public void Update(ApiAccessRestrictionsSnapshotDto snapshot)
    {
        _current = snapshot;
        _compiled = CompiledApiAccessRestrictionsSnapshot.Compile(snapshot);
    }
}