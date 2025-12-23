using System.Net;
using FluentAssertions;
using NSubstitute;
using snowcoreBlog.Backend.ApiAccessRestrictions.Entities;
using snowcoreBlog.Backend.ApiAccessRestrictions.Repositories.Marten;
using snowcoreBlog.Backend.ApiAccessRestrictions.Services;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Tests.Services;

public class ApiAccessRestrictionEvaluatorTests
{
    private readonly IIpRestrictionRepository _ipRepo = Substitute.For<IIpRestrictionRepository>();
    private readonly IRegionRestrictionRepository _regionRepo = Substitute.For<IRegionRestrictionRepository>();
    private readonly IApiAccessRuleRepository _ruleRepo = Substitute.For<IApiAccessRuleRepository>();
    private readonly IApiAccessResponseTemplateRepository _templateRepo = Substitute.For<IApiAccessResponseTemplateRepository>();

    private readonly ApiAccessRestrictionEvaluator _evaluator;

    public ApiAccessRestrictionEvaluatorTests()
    {
        _evaluator = new ApiAccessRestrictionEvaluator(_ipRepo, _regionRepo, _ruleRepo, _templateRepo);
    }

    [Fact]
    public async Task EvaluateAsync_WithNoRulesAndNoLegacyRestrictions_Allows()
    {
        _ruleRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);
        _ipRepo.GetAllAsync().Returns([]);
        _regionRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);

        var result = await _evaluator.EvaluateAsync(new CheckApiAccessRequestDto
        {
            Path = "/api/test",
            Method = "GET",
            Ip = "192.168.1.10",
            CountryCode = "US"
        });

        result.IsAllowed.Should().BeTrue();
    }

    [Fact]
    public async Task EvaluateAsync_WithMatchingRule_BlocksAndReturnsTemplate()
    {
        var templateId = Guid.NewGuid();
        _ruleRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns([
            new ApiAccessRuleEntity
            {
                Id = Guid.NewGuid(),
                Enabled = true,
                Priority = 10,
                Action = ApiAccessRestrictionActionDto.Block,
                Methods = ["GET"],
                PathPatterns = ["/api/secret/*"],
                ResponseTemplateId = templateId
            }
        ]);

        _templateRepo.GetByIdAsync(templateId, Arg.Any<CancellationToken>())
            .Returns(new ApiAccessResponseTemplateEntity
            {
                Id = templateId,
                StatusCode = 451,
                ContentType = "application/json",
                BodyJson = "{\"error\":\"blocked\"}",
                Reason = "Blocked by admin rule"
            });

        var result = await _evaluator.EvaluateAsync(new CheckApiAccessRequestDto
        {
            Path = "/api/secret/data",
            Method = "GET",
            Ip = "203.0.113.10"
        });

        result.IsAllowed.Should().BeFalse();
        result.StatusCode.Should().Be(451);
        result.BodyJson.Should().Contain("blocked");
        result.Reason.Should().Be("Blocked by admin rule");
    }

    [Fact]
    public async Task EvaluateAsync_WithCidrIpRule_MatchesAndBlocks()
    {
        _ruleRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns([
            new ApiAccessRuleEntity
            {
                Enabled = true,
                Priority = 1,
                Action = ApiAccessRestrictionActionDto.Block,
                IpRanges = ["10.0.0.0/24"],
                PathPatterns = ["/api/*"]
            }
        ]);

        var result = await _evaluator.EvaluateAsync(new CheckApiAccessRequestDto
        {
            Path = "/api/anything",
            Method = "POST",
            Ip = "10.0.0.42"
        });

        result.IsAllowed.Should().BeFalse();
        result.Action.Should().Be(ApiAccessRestrictionActionDto.Block);
    }

    [Fact]
    public async Task EvaluateAsync_WithLegacyIpRestriction_Blocks()
    {
        _ruleRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);
        _ipRepo.GetAllAsync().Returns([
            new IpRestrictionEntity
            {
                IsBlocked = true,
                IpRanges = ["192.168.1.0/24"]
            }
        ]);
        _regionRepo.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);

        var result = await _evaluator.EvaluateAsync(new CheckApiAccessRequestDto
        {
            Path = "/api/test",
            Method = "GET",
            Ip = IPAddress.Parse("192.168.1.10").ToString()
        });

        result.IsAllowed.Should().BeFalse();
        result.StatusCode.Should().Be(403);
    }
}