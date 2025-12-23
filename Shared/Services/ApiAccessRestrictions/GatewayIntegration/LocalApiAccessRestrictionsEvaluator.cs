using System.Net;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;

public interface ILocalApiAccessRestrictionsEvaluator
{
    CheckApiAccessResponseDto Evaluate(CompiledApiAccessRestrictionsSnapshot snapshot, CheckApiAccessRequestDto request);
}

public sealed class LocalApiAccessRestrictionsEvaluator : ILocalApiAccessRestrictionsEvaluator
{
    public CheckApiAccessResponseDto Evaluate(CompiledApiAccessRestrictionsSnapshot snapshot, CheckApiAccessRequestDto request)
    {
        // 1) Admin-defined API rules
        var ordered = snapshot.Rules
            .Select(r => r.Rule)
            .Where(r => r.Enabled)
            .Where(r => r.ExpiresAt is null || r.ExpiresAt > DateTimeOffset.UtcNow)
            .OrderByDescending(r => r.Priority)
            .ThenByDescending(r => r.CreatedAt)
            .ToList();

        foreach (var rule in ordered)
        {
            var compiled = snapshot.Rules.First(x => x.Rule.Id == rule.Id);
            if (!MatchesRule(compiled, request))
                continue;

            return ApplyRule(snapshot, rule);
        }

        // 2) Existing IP restrictions
        if (!string.IsNullOrWhiteSpace(request.Ip) && IPAddress.TryParse(request.Ip, out var ip))
        {
            if (IsBlockedByIp(snapshot.IpRestrictions, ip))
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
            var decision = EvaluateRegion(snapshot.RegionRestrictions, request.CountryCode!, request.Path);
            if (decision is not default(CheckApiAccessResponseDto))
                return decision;
        }

        return new CheckApiAccessResponseDto { IsAllowed = true };
    }

    private static CheckApiAccessResponseDto ApplyRule(CompiledApiAccessRestrictionsSnapshot snapshot, ApiAccessRuleSnapshotDto rule)
    {
        ApiAccessResponseTemplateSnapshotDto? template = null;
        if (rule.ResponseTemplateId is not null && snapshot.TemplatesById.TryGetValue(rule.ResponseTemplateId.Value, out var t))
        {
            template = t;
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

    private static CheckApiAccessResponseDto? EvaluateRegion(
        IReadOnlyList<CompiledApiAccessRestrictionsSnapshot.CompiledRegionRestriction> restrictions,
        string countryCode,
        string path)
    {
        var code = countryCode.Trim();
        foreach (var r in restrictions)
        {
            var entity = r.Restriction;
            if (entity.ExpiresAt is not null && entity.ExpiresAt <= DateTimeOffset.UtcNow)
                continue;

            if (!string.Equals(entity.RegionCode, code, StringComparison.OrdinalIgnoreCase))
                continue;

            if (entity.AffectedPaths.Count > 0 && !r.PathMatchers.Any(m => m.IsMatch(path)))
                continue;

            return entity.Level switch
            {
                ApiAccessRestrictionActionDto.RequireCaptcha => new CheckApiAccessResponseDto
                {
                    IsAllowed = false,
                    Action = ApiAccessRestrictionActionDto.RequireCaptcha,
                    Reason = "Captcha required for this region.",
                    StatusCode = StatusCodes.Status403Forbidden,
                    ContentType = "application/json"
                },
                ApiAccessRestrictionActionDto.AllowWithWarning => new CheckApiAccessResponseDto
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

        return default;
    }

    private static bool MatchesRule(CompiledApiAccessRestrictionsSnapshot.CompiledRule rule, CheckApiAccessRequestDto request)
    {
        var r = rule.Rule;

        if (r.Methods.Count > 0 && !r.Methods.Contains(request.Method, StringComparer.OrdinalIgnoreCase))
            return false;

        if (r.PathPatterns.Count > 0 && !rule.PathMatchers.Any(m => m.IsMatch(request.Path)))
            return false;

        if (r.Tags.Count > 0)
        {
            if (request.Tags is null || request.Tags.Count == 0)
                return false;

            if (!r.Tags.Intersect(request.Tags, StringComparer.OrdinalIgnoreCase).Any())
                return false;
        }

        if (r.RegionCodes.Count > 0)
        {
            if (string.IsNullOrWhiteSpace(request.CountryCode))
                return false;

            if (!r.RegionCodes.Contains(request.CountryCode.Trim(), StringComparer.OrdinalIgnoreCase))
                return false;
        }

        if (r.IpRanges.Count > 0)
        {
            if (string.IsNullOrWhiteSpace(request.Ip) || !IPAddress.TryParse(request.Ip, out var ip))
                return false;

            if (!rule.IpMatchers.Any(m => m.IsMatch(ip)))
                return false;
        }

        return true;
    }

    private static bool IsBlockedByIp(IReadOnlyList<CompiledApiAccessRestrictionsSnapshot.CompiledIpRestriction> restrictions, IPAddress ip)
    {
        foreach (var r in restrictions)
        {
            var entity = r.Restriction;
            if (!entity.IsBlocked)
                continue;

            if (entity.ExpiresAt is not null && entity.ExpiresAt <= DateTimeOffset.UtcNow)
                continue;

            if (r.IpMatchers.Any(m => m.IsMatch(ip)))
                return true;
        }

        return false;
    }
}