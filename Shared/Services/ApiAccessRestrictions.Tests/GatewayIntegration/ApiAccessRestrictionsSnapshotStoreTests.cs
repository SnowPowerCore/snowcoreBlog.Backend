using FluentAssertions;
using snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Tests.GatewayIntegration;

public class ApiAccessRestrictionsSnapshotStoreTests
{
    private static ApiAccessRestrictionsSnapshotDto Snapshot(Guid ruleId) => new()
    {
        GeneratedAt = DateTimeOffset.UtcNow,
        Rules =
        [
            new ApiAccessRuleSnapshotDto
            {
                Id = ruleId,
                Enabled = true,
                Priority = 1,
                Action = ApiAccessRestrictionActionDto.Block,
                CreatedAt = DateTimeOffset.UtcNow,
                PathPatterns = ["/api/articles"]
            }
        ],
        ResponseTemplates = [],
        IpRestrictions = [],
        RegionRestrictions = []
    };

    [Fact]
    public void Current_IsNullBeforeFirstUpdate()
    {
        var store = new ApiAccessRestrictionsSnapshotStore();

        store.Current.Should().BeNull();
        store.CurrentCompiled.Should().BeNull();
    }

    [Fact]
    public void Update_ExposesDtoAndCompiledFormAtomically()
    {
        var store = new ApiAccessRestrictionsSnapshotStore();
        var ruleId = Guid.NewGuid();

        store.Update(Snapshot(ruleId));

        store.Current.Should().NotBeNull();
        store.CurrentCompiled.Should().NotBeNull();
        store.CurrentCompiled!.RulesById.Should().ContainKey(ruleId);
        store.Current!.Rules.Single().Id.Should().Be(ruleId);
    }

    [Fact]
    public void Update_SwapsToNewerSnapshotConsistently()
    {
        var store = new ApiAccessRestrictionsSnapshotStore();
        store.Update(Snapshot(Guid.NewGuid()));
        var newRuleId = Guid.NewGuid();

        store.Update(Snapshot(newRuleId));

        store.CurrentCompiled!.RulesById.Keys.Should().ContainSingle().Which.Should().Be(newRuleId);
    }
}

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
