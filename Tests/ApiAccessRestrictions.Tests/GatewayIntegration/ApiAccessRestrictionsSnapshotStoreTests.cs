using FluentAssertions;
using snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Services;
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
