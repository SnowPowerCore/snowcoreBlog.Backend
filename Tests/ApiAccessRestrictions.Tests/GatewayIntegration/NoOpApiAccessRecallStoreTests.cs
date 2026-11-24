using FluentAssertions;
using snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Services;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Tests.GatewayIntegration;

public class NoOpApiAccessRecallStoreTests
{
    [Fact]
    public async Task TryGetAsync_NeverReturnsAnEntry()
    {
        var result = await NoOpApiAccessRecallStore.Instance.TryGetAsync("u:123", TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_IsANoOp()
    {
        await NoOpApiAccessRecallStore.Instance.SetAsync(
            "u:123",
            new ApiAccessRecallEntry(Guid.NewGuid(), ApiAccessRestrictionActionDto.Block, "reason", DateTimeOffset.UtcNow),
            TimeSpan.FromMinutes(1),
            TestContext.Current.CancellationToken);
    }
}
