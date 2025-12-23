using System.Net;
using System.Text.RegularExpressions;
using snowcoreBlog.Backend.ApiAccessRestrictions.Entities;
using snowcoreBlog.Backend.ApiAccessRestrictions.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Services;

public interface IApiAccessRestrictionEvaluator
{
    Task<CheckApiAccessResponseDto> EvaluateAsync(CheckApiAccessRequestDto request, CancellationToken ct = default);
}

public class ApiAccessRestrictionEvaluator(
    IIpRestrictionRepository ipRepo,
    IRegionRestrictionRepository regionRepo,
    IApiAccessRuleRepository ruleRepo,
    IApiAccessResponseTemplateRepository templateRepo) : IApiAccessRestrictionEvaluator
{
    public async Task<CheckApiAccessResponseDto> EvaluateAsync(CheckApiAccessRequestDto request, CancellationToken ct = default)
    {
        // 1) Admin-defined API rules
        var rules = await ruleRepo.GetAllAsync(ct);
        var ordered = rules
            .Where(r => r.Enabled)
            .Where(r => r.ExpiresAt is null || r.ExpiresAt > DateTimeOffset.UtcNow)
            .OrderByDescending(r => r.Priority)
            .ThenByDescending(r => r.CreatedAt)
            .ToList();

        foreach (var rule in ordered)
        {
            if (!MatchesRule(rule, request))
                continue;

            return await DenyFromRuleAsync(rule, request, ct);
        }

        // 2) Existing IP restrictions
        if (!string.IsNullOrWhiteSpace(request.Ip) && IPAddress.TryParse(request.Ip, out var ip))
        {
            var ipRestrictions = await ipRepo.GetAllAsync();
            if (IsBlockedByIp(ipRestrictions, ip))
            {
                return new CheckApiAccessResponseDto
                {
                    IsAllowed = false,
                    Action = ApiAccessRestrictionActionDto.Block,
                    Reason = "Access restricted by IP policy.",
                    StatusCode = StatusCodes.Status403Forbidden,
                    ContentType = "application/json",
                    BodyJson = null
                };
            }
        }

        // 3) Existing region restrictions
        if (!string.IsNullOrWhiteSpace(request.CountryCode))
        {
            var regionRestrictions = await regionRepo.GetAllAsync(ct);
            var decision = EvaluateRegion(regionRestrictions, request.CountryCode!, request.Path);
            if (decision is not null)
            {
                return decision;
            }
        }

        return new CheckApiAccessResponseDto { IsAllowed = true };
    }

    private async Task<CheckApiAccessResponseDto> DenyFromRuleAsync(ApiAccessRuleEntity rule, CheckApiAccessRequestDto request, CancellationToken ct)
    {
        ApiAccessResponseTemplateEntity? template = null;
        if (rule.ResponseTemplateId is not null)
        {
            template = await templateRepo.GetByIdAsync(rule.ResponseTemplateId.Value, ct);
        }

        var status = template?.StatusCode ?? StatusCodes.Status403Forbidden;
        var contentType = template?.ContentType ?? "application/json";
        var reason = template?.Reason ?? rule.Description ?? "Access restricted by policy.";
        var bodyJson = template?.BodyJson;

        var isAllowed = rule.Action == ApiAccessRestrictionActionDto.AllowWithWarning;

        return new CheckApiAccessResponseDto
        {
            IsAllowed = isAllowed,
            MatchedRuleId = rule.Id,
            Action = rule.Action,
            Reason = reason,
            StatusCode = isAllowed ? null : status,
            ContentType = isAllowed ? null : contentType,
            BodyJson = isAllowed ? null : bodyJson
        };
    }

    private static CheckApiAccessResponseDto? EvaluateRegion(IReadOnlyList<RegionRestrictionEntity> restrictions, string countryCode, string path)
    {
        var code = countryCode.Trim();
        foreach (var r in restrictions)
        {
            if (r.ExpiresAt is not null && r.ExpiresAt <= DateTimeOffset.UtcNow)
                continue;

            if (!string.Equals(r.RegionCode, code, StringComparison.OrdinalIgnoreCase))
                continue;

            if (r.AffectedPaths.Count > 0 && !r.AffectedPaths.Any(p => PathMatches(p, path)))
                continue;

            return r.Level switch
            {
                RestrictionLevel.RequireCaptcha => new CheckApiAccessResponseDto
                {
                    IsAllowed = false,
                    Action = ApiAccessRestrictionActionDto.RequireCaptcha,
                    Reason = "Captcha required for this region.",
                    StatusCode = StatusCodes.Status403Forbidden,
                    ContentType = "application/json"
                },
                RestrictionLevel.AllowWithWarning => new CheckApiAccessResponseDto
                {
                    IsAllowed = true,
                    Action = ApiAccessRestrictionActionDto.AllowWithWarning,
                    Reason = "Access allowed with warning for this region."
                },
                _ => new CheckApiAccessResponseDto
                {
                    IsAllowed = false,
                    Action = ApiAccessRestrictionActionDto.Block,
                    Reason = "Access blocked for this region.",
                    StatusCode = StatusCodes.Status403Forbidden,
                    ContentType = "application/json"
                }
            };
        }

        return null;
    }

    private static bool MatchesRule(ApiAccessRuleEntity rule, CheckApiAccessRequestDto request)
    {
        if (rule.Methods.Count > 0 && !rule.Methods.Contains(request.Method, StringComparer.OrdinalIgnoreCase))
            return false;

        if (rule.PathPatterns.Count > 0 && !rule.PathPatterns.Any(p => PathMatches(p, request.Path)))
            return false;

        if (rule.Tags.Count > 0)
        {
            if (request.Tags is null || request.Tags.Count == 0)
                return false;

            if (!rule.Tags.Intersect(request.Tags, StringComparer.OrdinalIgnoreCase).Any())
                return false;
        }

        if (rule.RegionCodes.Count > 0)
        {
            if (string.IsNullOrWhiteSpace(request.CountryCode))
                return false;

            if (!rule.RegionCodes.Contains(request.CountryCode.Trim(), StringComparer.OrdinalIgnoreCase))
                return false;
        }

        if (rule.IpRanges.Count > 0)
        {
            if (string.IsNullOrWhiteSpace(request.Ip) || !IPAddress.TryParse(request.Ip, out var ip))
                return false;

            if (!rule.IpRanges.Any(r => IpMatches(r, ip)))
                return false;
        }

        return true;
    }

    private static bool IsBlockedByIp(IReadOnlyList<IpRestrictionEntity> restrictions, IPAddress ip)
    {
        var ipv4 = ip.MapToIPv4();

        foreach (var r in restrictions.Where(x => x.IsBlocked))
        {
            if (r.ExpiresAt is not null && r.ExpiresAt <= DateTimeOffset.UtcNow)
                continue;

            foreach (var range in r.IpRanges)
            {
                if (string.IsNullOrWhiteSpace(range))
                    continue;

                if (IpMatches(range, ipv4))
                    return true;
            }
        }

        return false;
    }

    private static bool PathMatches(string pattern, string path)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            return false;

        if (pattern.StartsWith("regex:", StringComparison.OrdinalIgnoreCase))
        {
            var rx = pattern["regex:".Length..];
            return Regex.IsMatch(path, rx, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        if (pattern.Contains('*'))
        {
            var escaped = Regex.Escape(pattern).Replace("\\*", ".*");
            return Regex.IsMatch(path, $"^{escaped}$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        return path.StartsWith(pattern, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IpMatches(string range, IPAddress ip)
    {
        var trimmed = range.Trim();

        // CIDR: 10.0.0.0/24
        var slash = trimmed.IndexOf('/');
        if (slash > 0)
        {
            var ipPart = trimmed[..slash];
            var maskPart = trimmed[(slash + 1)..];
            if (IPAddress.TryParse(ipPart, out var networkIp) && int.TryParse(maskPart, out var prefixLength))
            {
                return CidrContains(networkIp.MapToIPv4(), prefixLength, ip.MapToIPv4());
            }
        }

        // Range: 10.0.0.1-10.0.0.10
        var dash = trimmed.IndexOf('-');
        if (dash > 0)
        {
            var start = trimmed[..dash];
            var end = trimmed[(dash + 1)..];
            if (IPAddress.TryParse(start, out var startIp) && IPAddress.TryParse(end, out var endIp))
            {
                var s = ToUInt32(startIp.MapToIPv4());
                var e = ToUInt32(endIp.MapToIPv4());
                var x = ToUInt32(ip.MapToIPv4());
                return x >= Math.Min(s, e) && x <= Math.Max(s, e);
            }
        }

        // Exact IP
        if (IPAddress.TryParse(trimmed, out var exact))
            return exact.MapToIPv4().Equals(ip.MapToIPv4());

        // Back-compat: prefix match like "192.168.1"
        return ip.MapToIPv4().ToString().StartsWith(trimmed, StringComparison.OrdinalIgnoreCase);
    }

    private static bool CidrContains(IPAddress network, int prefixLength, IPAddress address)
    {
        if (prefixLength is < 0 or > 32)
            return false;

        var mask = prefixLength == 0 ? 0u : uint.MaxValue << (32 - prefixLength);
        var net = ToUInt32(network);
        var addr = ToUInt32(address);
        return (net & mask) == (addr & mask);
    }

    private static uint ToUInt32(IPAddress ip)
    {
        var bytes = ip.GetAddressBytes();
        if (bytes.Length != 4)
            throw new NotSupportedException("Only IPv4 is supported for range checks.");

        return ((uint)bytes[0] << 24) | ((uint)bytes[1] << 16) | ((uint)bytes[2] << 8) | bytes[3];
    }
}