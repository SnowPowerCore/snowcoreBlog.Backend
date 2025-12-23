using System.Net;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Services;

public interface IRequestRestrictionService
{
    Task<bool> IsAllowedAsync(IPAddress ip, string path, CancellationToken ct = default);
}