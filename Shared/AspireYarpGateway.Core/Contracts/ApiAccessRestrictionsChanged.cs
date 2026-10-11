namespace snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;

/// <summary>
/// Published by the ApiAccessRestrictions service whenever a rule/template changes so
/// the gateway refreshes its in-memory restrictions snapshot immediately instead of
/// waiting for the periodic poll.
/// </summary>
public record ApiAccessRestrictionsChanged(DateTimeOffset ChangedAtUtc);
