using StackExchange.Redis;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;

/// <summary>
/// Redis-backed recall store (hash fields, no JSON dependency) keyed by
/// <c>api-access:recall:{clientId}</c>. Shared with the gateway so a restriction
/// decided after a response was served is enforced on the client's next request.
/// </summary>
public sealed class RedisApiAccessRecallStore(IConnectionMultiplexer connectionMultiplexer) : IApiAccessRecallStore
{
    private const string KeyPrefix = "api-access:recall:";

    public async Task<ApiAccessRecallEntry?> TryGetAsync(string clientId, CancellationToken cancellationToken)
    {
        var database = connectionMultiplexer.GetDatabase();
        var entries = await database.HashGetAllAsync(KeyPrefix + clientId).WaitAsync(cancellationToken);

        if (entries.Length == 0)
            return null;

        DateTimeOffset createdAtUtc = default;
        Guid? ruleId = null;
        var action = ApiAccessRestrictionActionDto.Block;
        string? reason = null;

        foreach (var entry in entries)
        {
            switch (entry.Name.ToString())
            {
                case "ruleId" when Guid.TryParse((ReadOnlySpan<char>)entry.Value, out var parsedRuleId):
                    ruleId = parsedRuleId;
                    break;
                case "action" when Enum.TryParse<ApiAccessRestrictionActionDto>(entry.Value, out var parsedAction):
                    action = parsedAction;
                    break;
                case "reason":
                    reason = entry.Value.IsNull ? null : entry.Value.ToString();
                    break;
                case "createdAt" when DateTimeOffset.TryParse(entry.Value, out var parsedCreatedAt):
                    createdAtUtc = parsedCreatedAt;
                    break;
            }
        }

        return new ApiAccessRecallEntry(ruleId, action, reason, createdAtUtc);
    }

    public async Task SetAsync(string clientId, ApiAccessRecallEntry entry, TimeSpan timeToLive, CancellationToken cancellationToken)
    {
        var database = connectionMultiplexer.GetDatabase();
        var key = KeyPrefix + clientId;

        var hashEntries = new[]
        {
            new HashEntry("ruleId", entry.RuleId?.ToString() ?? RedisValue.Null),
            new HashEntry("action", entry.Action.ToString()),
            new HashEntry("reason", entry.Reason ?? RedisValue.Null),
            new HashEntry("createdAt", entry.CreatedAtUtc.ToString("O"))
        };

        await database.HashSetAsync(key, hashEntries).WaitAsync(cancellationToken);
        await database.KeyExpireAsync(key, timeToLive).WaitAsync(cancellationToken);
    }
}
