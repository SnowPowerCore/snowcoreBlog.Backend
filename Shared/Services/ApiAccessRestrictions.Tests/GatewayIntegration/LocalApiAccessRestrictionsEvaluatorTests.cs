using FluentAssertions;
using snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Tests.GatewayIntegration;

public class LocalApiAccessRestrictionsEvaluatorTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.UtcNow;

    // Snapshot DTOs have init-only setters: mutations go through `with` expressions.
    private static ApiAccessRuleSnapshotDto Rule(Func<ApiAccessRuleSnapshotDto, ApiAccessRuleSnapshotDto>? mutate = null)
    {
        var baseRule = new ApiAccessRuleSnapshotDto
        {
            Id = Guid.NewGuid(),
            Enabled = true,
            Priority = 10,
            Action = ApiAccessRestrictionActionDto.Block,
            CreatedAt = Now
        };

        return mutate?.Invoke(baseRule) ?? baseRule;
    }

    private static CompiledApiAccessRestrictionsSnapshot Compile(params ApiAccessRuleSnapshotDto[] rules) =>
        CompiledApiAccessRestrictionsSnapshot.Compile(new ApiAccessRestrictionsSnapshotDto
        {
            GeneratedAt = Now,
            Rules = rules,
            ResponseTemplates = [],
            IpRestrictions = [],
            RegionRestrictions = []
        });

    private static CheckApiAccessRequestDto Request(string path = "/api/articles/123",
                                                    string method = "GET",
                                                    string? ip = "203.0.113.10",
                                                    string? country = "US",
                                                    IReadOnlyCollection<string>? tags = null) => new()
    {
        Path = path,
        Method = method,
        Ip = ip,
        CountryCode = country,
        Tags = tags
    };

    private readonly LocalApiAccessRestrictionsEvaluator _evaluator = new();

    [Fact]
    public void Evaluate_WithNoRules_Allows()
    {
        var decision = _evaluator.Evaluate(Compile(), Request());

        decision.IsAllowed.Should().BeTrue();
        decision.Action.Should().BeNull();
    }

    [Fact]
    public void Evaluate_WithPrefixPathRule_BlocksAndUsesRuleDescriptionAsReason()
    {
        var snapshot = Compile(Rule(r => r with
        {
            Description = "Maintenance window",
            PathPatterns = ["/api/articles"]
        }));

        var decision = _evaluator.Evaluate(snapshot, Request());

        decision.IsAllowed.Should().BeFalse();
        decision.Action.Should().Be(ApiAccessRestrictionActionDto.Block);
        decision.Reason.Should().Be("Maintenance window");
        decision.StatusCode.Should().Be(403);
        decision.MatchedRuleId.Should().Be(snapshot.RulesById.Values.Single().Rule.Id);
    }

    [Theory]
    [InlineData("/api/secret/deep/nested", true)]
    [InlineData("/api/secret", false)]
    public void Evaluate_WithWildcardPattern_MatchesOnlyUnderPrefix(string path, bool shouldBlock)
    {
        var snapshot = Compile(Rule(r => r with { PathPatterns = ["/api/secret/*"] }));

        var decision = _evaluator.Evaluate(snapshot, Request(path));

        decision.IsAllowed.Should().Be(!shouldBlock);
    }

    [Fact]
    public void Evaluate_WithRegexPattern_BlocksMatchingPath()
    {
        var snapshot = Compile(Rule(r => r with { PathPatterns = ["regex:^/api/admin/.*$"] }));

        _evaluator.Evaluate(snapshot, Request("/api/admin/users")).IsAllowed.Should().BeFalse();
        _evaluator.Evaluate(snapshot, Request("/api/articles")).IsAllowed.Should().BeTrue();
    }

    [Fact]
    public void Evaluate_WithMethodMismatch_Allows()
    {
        var snapshot = Compile(Rule(r => r with
        {
            Methods = ["POST"],
            PathPatterns = ["/api/articles"]
        }));

        _evaluator.Evaluate(snapshot, Request(method: "GET")).IsAllowed.Should().BeTrue();
        _evaluator.Evaluate(snapshot, Request(method: "POST")).IsAllowed.Should().BeFalse();
    }

    [Fact]
    public void Evaluate_WithDisabledRule_Allows()
    {
        var snapshot = Compile(Rule(r => r with
        {
            Enabled = false,
            PathPatterns = ["/api/articles"]
        }));

        _evaluator.Evaluate(snapshot, Request()).IsAllowed.Should().BeTrue();
    }

    [Fact]
    public void Evaluate_WithExpiredRule_Allows()
    {
        var snapshot = Compile(Rule(r => r with
        {
            PathPatterns = ["/api/articles"],
            ExpiresAt = Now.AddMinutes(-1)
        }));

        _evaluator.Evaluate(snapshot, Request()).IsAllowed.Should().BeTrue();
    }

    [Fact]
    public void Evaluate_WithHigherPriorityRule_Wins()
    {
        var allowedWithWarning = Rule(r => r with
        {
            Priority = 100,
            Action = ApiAccessRestrictionActionDto.AllowWithWarning,
            PathPatterns = ["/api/articles"]
        });
        var blocking = Rule(r => r with
        {
            Priority = 1,
            PathPatterns = ["/api/articles"]
        });
        var snapshot = CompiledApiAccessRestrictionsSnapshot.Compile(new ApiAccessRestrictionsSnapshotDto
        {
            GeneratedAt = Now,
            Rules = [blocking, allowedWithWarning],
            ResponseTemplates = [],
            IpRestrictions = [],
            RegionRestrictions = []
        });

        var decision = _evaluator.Evaluate(snapshot, Request());

        decision.IsAllowed.Should().BeTrue();
        decision.Action.Should().Be(ApiAccessRestrictionActionDto.AllowWithWarning);
        decision.StatusCode.Should().BeNull();
        decision.BodyJson.Should().BeNull();
    }

    [Fact]
    public void Evaluate_WithResponseTemplate_AppliesTemplateResponse()
    {
        var templateId = Guid.NewGuid();
        var template = new ApiAccessResponseTemplateSnapshotDto
        {
            Id = templateId,
            StatusCode = 451,
            ContentType = "application/json",
            BodyJson = """{"error":"blocked"}""",
            Reason = "Legal hold",
            CreatedAt = Now
        };
        var rule = Rule(r => r with
        {
            PathPatterns = ["/api/articles"],
            ResponseTemplateId = templateId
        });
        var snapshot = CompiledApiAccessRestrictionsSnapshot.Compile(new ApiAccessRestrictionsSnapshotDto
        {
            GeneratedAt = Now,
            Rules = [rule],
            ResponseTemplates = [template],
            IpRestrictions = [],
            RegionRestrictions = []
        });

        var decision = _evaluator.Evaluate(snapshot, Request());

        decision.IsAllowed.Should().BeFalse();
        decision.StatusCode.Should().Be(451);
        decision.ContentType.Should().Be("application/json");
        decision.BodyJson.Should().Contain("blocked");
        decision.Reason.Should().Be("Legal hold");
    }

    [Fact]
    public void Evaluate_WithRequireCaptchaRule_IsDeniedWithCaptchaAction()
    {
        var snapshot = Compile(Rule(r => r with
        {
            Action = ApiAccessRestrictionActionDto.RequireCaptcha,
            PathPatterns = ["/api/articles"]
        }));

        var decision = _evaluator.Evaluate(snapshot, Request());

        decision.IsAllowed.Should().BeFalse();
        decision.Action.Should().Be(ApiAccessRestrictionActionDto.RequireCaptcha);
        decision.StatusCode.Should().Be(403);
    }

    [Fact]
    public void Evaluate_WithTagRule_RequiresTagIntersection()
    {
        var snapshot = Compile(Rule(r => r with
        {
            Tags = ["reader"],
            PathPatterns = ["/api/articles"]
        }));

        _evaluator.Evaluate(snapshot, Request(tags: ["reader"])).IsAllowed.Should().BeFalse();
        _evaluator.Evaluate(snapshot, Request(tags: ["admin"])).IsAllowed.Should().BeTrue();
        _evaluator.Evaluate(snapshot, Request(tags: [])).IsAllowed.Should().BeTrue();
    }

    [Fact]
    public void Evaluate_WithRegionCodeRule_MatchesOnlyThatCountry()
    {
        var snapshot = Compile(Rule(r => r with
        {
            RegionCodes = ["RU"],
            PathPatterns = ["/api/articles"]
        }));

        _evaluator.Evaluate(snapshot, Request(country: "RU")).IsAllowed.Should().BeFalse();
        _evaluator.Evaluate(snapshot, Request(country: "US")).IsAllowed.Should().BeTrue();
        _evaluator.Evaluate(snapshot, Request(country: null)).IsAllowed.Should().BeTrue();
    }

    [Fact]
    public void Evaluate_WithCidrIpRange_BlocksMatchingIp()
    {
        var snapshot = Compile(Rule(r => r with { IpRanges = ["203.0.113.0/24"] }));

        _evaluator.Evaluate(snapshot, Request(ip: "203.0.113.42")).IsAllowed.Should().BeFalse();
        _evaluator.Evaluate(snapshot, Request(ip: "203.0.114.42")).IsAllowed.Should().BeTrue();
    }

    [Fact]
    public void Evaluate_WithLegacyIpRestriction_Blocks()
    {
        var snapshot = CompiledApiAccessRestrictionsSnapshot.Compile(new ApiAccessRestrictionsSnapshotDto
        {
            GeneratedAt = Now,
            Rules = [],
            ResponseTemplates = [],
            IpRestrictions =
            [
                new IpRestrictionSnapshotDto
                {
                    Id = Guid.NewGuid(),
                    IsBlocked = true,
                    CreatedAt = Now,
                    IpRanges = ["192.168.1.0/24"]
                }
            ],
            RegionRestrictions = []
        });

        var decision = _evaluator.Evaluate(snapshot, Request(ip: "192.168.1.10"));

        decision.IsAllowed.Should().BeFalse();
        decision.StatusCode.Should().Be(403);
    }

    [Fact]
    public void Evaluate_WithRegionRestrictionScopedToPaths_BlocksOnlyAffectedPaths()
    {
        var snapshot = CompiledApiAccessRestrictionsSnapshot.Compile(new ApiAccessRestrictionsSnapshotDto
        {
            GeneratedAt = Now,
            Rules = [],
            ResponseTemplates = [],
            IpRestrictions = [],
            RegionRestrictions =
            [
                new RegionRestrictionSnapshotDto
                {
                    Id = Guid.NewGuid(),
                    RegionCode = "DE",
                    Level = ApiAccessRestrictionActionDto.Block,
                    CreatedAt = Now,
                    AffectedPaths = ["/api/articles"]
                }
            ]
        });

        _evaluator.Evaluate(snapshot, Request(path: "/api/articles/5", country: "DE")).IsAllowed.Should().BeFalse();
        _evaluator.Evaluate(snapshot, Request(path: "/api/authors", country: "DE")).IsAllowed.Should().BeTrue();
        _evaluator.Evaluate(snapshot, Request(path: "/api/articles/5", country: "FR")).IsAllowed.Should().BeTrue();
    }

    [Fact]
    public void Evaluate_RulesById_LooksUpWithoutLinearScan()
    {
        var rules = Enumerable.Range(0, 500)
            .Select(i => Rule(r => r with
            {
                Priority = i,
                PathPatterns = ["/api/never-matches"]
            }))
            .ToArray();

        var snapshot = CompiledApiAccessRestrictionsSnapshot.Compile(new ApiAccessRestrictionsSnapshotDto
        {
            GeneratedAt = Now,
            Rules = rules,
            ResponseTemplates = [],
            IpRestrictions = [],
            RegionRestrictions = []
        });

        snapshot.RulesById.Should().HaveCount(rules.Length);

        var decision = _evaluator.Evaluate(snapshot, Request());
        decision.IsAllowed.Should().BeTrue();
    }
}
