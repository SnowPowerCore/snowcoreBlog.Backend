using System.Net;
using System.Text.RegularExpressions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;

public sealed class CompiledApiAccessRestrictionsSnapshot
{
    public required DateTimeOffset GeneratedAt { get; init; }

    public required IReadOnlyList<CompiledRule> Rules { get; init; }

    public required IReadOnlyDictionary<Guid, ApiAccessResponseTemplateSnapshotDto> TemplatesById { get; init; }

    public required IReadOnlyList<CompiledIpRestriction> IpRestrictions { get; init; }

    public required IReadOnlyList<CompiledRegionRestriction> RegionRestrictions { get; init; }

    public static CompiledApiAccessRestrictionsSnapshot Compile(ApiAccessRestrictionsSnapshotDto snapshot)
    {
        var templatesById = snapshot.ResponseTemplates
            .GroupBy(t => t.Id)
            .ToDictionary(g => g.Key, g => g.Last());

        var rules = snapshot.Rules
            .Select(CompiledRule.Compile)
            .ToList();

        var ipRestrictions = snapshot.IpRestrictions
            .Select(CompiledIpRestriction.Compile)
            .ToList();

        var regionRestrictions = snapshot.RegionRestrictions
            .Select(CompiledRegionRestriction.Compile)
            .ToList();

        return new CompiledApiAccessRestrictionsSnapshot
        {
            GeneratedAt = snapshot.GeneratedAt,
            TemplatesById = templatesById,
            Rules = rules,
            IpRestrictions = ipRestrictions,
            RegionRestrictions = regionRestrictions
        };
    }

    public sealed class CompiledRule
    {
        public required ApiAccessRuleSnapshotDto Rule { get; init; }

        public required IReadOnlyList<PathMatcher> PathMatchers { get; init; }

        public required IReadOnlyList<IpMatcher> IpMatchers { get; init; }

        public static CompiledRule Compile(ApiAccessRuleSnapshotDto rule)
        {
            var pathMatchers = rule.PathPatterns.Select(PathMatcher.Compile).ToList();
            var ipMatchers = rule.IpRanges.Select(IpMatcher.Compile).ToList();

            return new CompiledRule
            {
                Rule = rule,
                PathMatchers = pathMatchers,
                IpMatchers = ipMatchers
            };
        }
    }

    public sealed class CompiledIpRestriction
    {
        public required IpRestrictionSnapshotDto Restriction { get; init; }

        public required IReadOnlyList<IpMatcher> IpMatchers { get; init; }

        public static CompiledIpRestriction Compile(IpRestrictionSnapshotDto restriction)
        {
            var matchers = restriction.IpRanges.Select(IpMatcher.Compile).ToList();
            return new CompiledIpRestriction { Restriction = restriction, IpMatchers = matchers };
        }
    }

    public sealed class CompiledRegionRestriction
    {
        public required RegionRestrictionSnapshotDto Restriction { get; init; }

        public required IReadOnlyList<PathMatcher> PathMatchers { get; init; }

        public static CompiledRegionRestriction Compile(RegionRestrictionSnapshotDto restriction)
        {
            var matchers = restriction.AffectedPaths.Select(PathMatcher.Compile).ToList();
            return new CompiledRegionRestriction { Restriction = restriction, PathMatchers = matchers };
        }
    }

    public abstract class PathMatcher
    {
        public static PathMatcher Compile(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                return new AlwaysFalseMatcher();

            if (pattern.StartsWith("regex:", StringComparison.OrdinalIgnoreCase))
            {
                var rx = pattern["regex:".Length..];
                return new RegexMatcher(new Regex(rx, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled));
            }

            if (pattern.Contains('*'))
            {
                var escaped = Regex.Escape(pattern).Replace("\\*", ".*");
                return new RegexMatcher(new Regex($"^{escaped}$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled));
            }

            return new PrefixMatcher(pattern);
        }

        public abstract bool IsMatch(string path);

        private sealed class AlwaysFalseMatcher : PathMatcher
        {
            public override bool IsMatch(string path) => false;
        }

        private sealed class PrefixMatcher(string prefix) : PathMatcher
        {
            public override bool IsMatch(string path) => path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }

        private sealed class RegexMatcher(Regex regex) : PathMatcher
        {
            public override bool IsMatch(string path) => regex.IsMatch(path);
        }
    }

    public abstract class IpMatcher
    {
        public static IpMatcher Compile(string range)
        {
            var trimmed = (range ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                return new AlwaysFalseIpMatcher();

            var slash = trimmed.IndexOf('/');
            if (slash > 0)
            {
                var ipPart = trimmed[..slash];
                var maskPart = trimmed[(slash + 1)..];
                if (IPAddress.TryParse(ipPart, out var networkIp) && int.TryParse(maskPart, out var prefixLength))
                {
                    var network = ToUInt32(networkIp.MapToIPv4());
                    var mask = prefixLength == 0 ? 0u : uint.MaxValue << (32 - prefixLength);
                    var networkMasked = network & mask;
                    return new CidrIpMatcher(networkMasked, mask);
                }
            }

            var dash = trimmed.IndexOf('-');
            if (dash > 0)
            {
                var start = trimmed[..dash];
                var end = trimmed[(dash + 1)..];
                if (IPAddress.TryParse(start, out var startIp) && IPAddress.TryParse(end, out var endIp))
                {
                    var s = ToUInt32(startIp.MapToIPv4());
                    var e = ToUInt32(endIp.MapToIPv4());
                    var min = Math.Min(s, e);
                    var max = Math.Max(s, e);
                    return new RangeIpMatcher(min, max);
                }
            }

            if (IPAddress.TryParse(trimmed, out var exact))
                return new ExactIpMatcher(ToUInt32(exact.MapToIPv4()));

            return new PrefixStringIpMatcher(trimmed);
        }

        public abstract bool IsMatch(IPAddress ip);

        private static uint ToUInt32(IPAddress ipv4)
        {
            var bytes = ipv4.GetAddressBytes();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        private sealed class AlwaysFalseIpMatcher : IpMatcher
        {
            public override bool IsMatch(IPAddress ip) => false;
        }

        private sealed class CidrIpMatcher(uint networkMasked, uint mask) : IpMatcher
        {
            public override bool IsMatch(IPAddress ip)
            {
                var value = ToUInt32(ip.MapToIPv4());
                return (value & mask) == networkMasked;
            }
        }

        private sealed class RangeIpMatcher(uint min, uint max) : IpMatcher
        {
            public override bool IsMatch(IPAddress ip)
            {
                var value = ToUInt32(ip.MapToIPv4());
                return value >= min && value <= max;
            }
        }

        private sealed class ExactIpMatcher(uint exact) : IpMatcher
        {
            public override bool IsMatch(IPAddress ip) => ToUInt32(ip.MapToIPv4()) == exact;
        }

        private sealed class PrefixStringIpMatcher(string prefix) : IpMatcher
        {
            public override bool IsMatch(IPAddress ip) => ip.MapToIPv4().ToString().StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }
    }
}