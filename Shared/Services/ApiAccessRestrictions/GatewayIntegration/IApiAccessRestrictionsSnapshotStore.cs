using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;

public interface IApiAccessRestrictionsSnapshotStore
{
    ApiAccessRestrictionsSnapshotDto? Current { get; }

    CompiledApiAccessRestrictionsSnapshot? CurrentCompiled { get; }

    void Update(ApiAccessRestrictionsSnapshotDto snapshot);
}