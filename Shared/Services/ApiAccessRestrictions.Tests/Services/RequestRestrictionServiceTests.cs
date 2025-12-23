using System.Net;
using FluentAssertions;
using NSubstitute;
using snowcoreBlog.Backend.ApiAccessRestrictions.Entities;
using snowcoreBlog.Backend.ApiAccessRestrictions.Repositories.Marten;
using snowcoreBlog.Backend.ApiAccessRestrictions.Services;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Tests.Services;

public class RequestRestrictionServiceTests
{
    private readonly IIpRestrictionRepository _ipRepo;
    private readonly RequestRestrictionService _service;

    public RequestRestrictionServiceTests()
    {
        _ipRepo = Substitute.For<IIpRestrictionRepository>();
        _service = new RequestRestrictionService(_ipRepo);
    }

    [Fact]
    public async Task IsAllowedAsync_WithNoRestrictions_ShouldReturnTrue()
    {
        // Arrange
        _ipRepo.GetAllAsync().Returns([]);
        var ip = IPAddress.Parse("192.168.1.1");

        // Act
        var result = await _service.IsAllowedAsync(ip, "/api/test");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsAllowedAsync_WithBlockedIpMatch_ShouldReturnFalse()
    {
        // Arrange
        var restrictions = new List<IpRestrictionEntity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                IsBlocked = true,
                IpRanges = ["192.168.1"]
            }
        };
        _ipRepo.GetAllAsync().Returns(restrictions);
        var ip = IPAddress.Parse("192.168.1.100");

        // Act
        var result = await _service.IsAllowedAsync(ip, "/api/test");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsAllowedAsync_WithExactIpMatch_ShouldReturnFalse()
    {
        // Arrange
        var restrictions = new List<IpRestrictionEntity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                IsBlocked = true,
                IpRanges = ["192.168.1.100"]
            }
        };
        _ipRepo.GetAllAsync().Returns(restrictions);
        var ip = IPAddress.Parse("192.168.1.100");

        // Act
        var result = await _service.IsAllowedAsync(ip, "/api/test");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsAllowedAsync_WithNonBlockedRestriction_ShouldReturnTrue()
    {
        // Arrange
        var restrictions = new List<IpRestrictionEntity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                IsBlocked = false,
                IpRanges = ["192.168.1"]
            }
        };
        _ipRepo.GetAllAsync().Returns(restrictions);
        var ip = IPAddress.Parse("192.168.1.100");

        // Act
        var result = await _service.IsAllowedAsync(ip, "/api/test");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsAllowedAsync_WithDifferentIp_ShouldReturnTrue()
    {
        // Arrange
        var restrictions = new List<IpRestrictionEntity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                IsBlocked = true,
                IpRanges = ["10.0.0"]
            }
        };
        _ipRepo.GetAllAsync().Returns(restrictions);
        var ip = IPAddress.Parse("192.168.1.100");

        // Act
        var result = await _service.IsAllowedAsync(ip, "/api/test");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsAllowedAsync_WithEmptyIpRange_ShouldReturnTrue()
    {
        // Arrange
        var restrictions = new List<IpRestrictionEntity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                IsBlocked = true,
                IpRanges = ["", "   "]
            }
        };
        _ipRepo.GetAllAsync().Returns(restrictions);
        var ip = IPAddress.Parse("192.168.1.100");

        // Act
        var result = await _service.IsAllowedAsync(ip, "/api/test");

        // Assert
        result.Should().BeTrue();
    }
}