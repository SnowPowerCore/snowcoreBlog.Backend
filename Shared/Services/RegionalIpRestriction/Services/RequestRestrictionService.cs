using System.Net;
using snowcoreBlog.Backend.RegionalIpRestriction.Repositories.Marten;

namespace snowcoreBlog.Backend.RegionalIpRestriction.Services;

public class RequestRestrictionService(IIpRestrictionRepository ipRepo) : IRequestRestrictionService
{

    // Very small proof-of-concept: checks if any restriction contains the remote IP as substring.
    // Replace with CIDR-aware check and GeoIP lookup in future iterations.
    public async Task<bool> IsAllowedAsync(IPAddress ip, string path, CancellationToken ct = default)
    {
        var list = await ipRepo.GetAllAsync();

        var s = ip.MapToIPv4().ToString();

        foreach (var r in list.Where(x => x.IsBlocked))
        {
            foreach (var rng in r.IpRanges)
            {
                if (string.IsNullOrWhiteSpace(rng))
                    continue;
                if (s.StartsWith(rng, StringComparison.OrdinalIgnoreCase) || s == rng)
                    return false;
            }
        }

        // For now allow if no match
        return true;
    }
}