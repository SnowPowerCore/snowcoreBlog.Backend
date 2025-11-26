using System.Net;

namespace snowcoreBlog.Backend.RegionalIpRestriction.Services;

public interface IRequestRestrictionService
{
    Task<bool> IsAllowedAsync(IPAddress ip, string path, CancellationToken ct = default);
}
