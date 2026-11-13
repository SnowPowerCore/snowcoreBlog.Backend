using snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Services;

public interface IApiAccessRestrictionsSnapshotStore
{
    ApiAccessRestrictionsSnapshotDto? Current { get; }

    CompiledApiAccessRestrictionsSnapshot? CurrentCompiled { get; }

    void Update(ApiAccessRestrictionsSnapshotDto snapshot);
}