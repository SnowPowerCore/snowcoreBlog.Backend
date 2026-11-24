using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Services;

public interface IApiAccessRecallStore
{
    Task<ApiAccessRecallEntry?> TryGetAsync(string clientId, CancellationToken cancellationToken);

    Task SetAsync(string clientId, ApiAccessRecallEntry entry, TimeSpan timeToLive, CancellationToken cancellationToken);
}