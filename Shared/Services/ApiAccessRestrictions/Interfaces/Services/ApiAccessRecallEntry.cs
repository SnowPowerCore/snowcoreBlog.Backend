using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Services;

/// <summary>
/// A restriction decision produced after a response has already been served
/// (fire-and-forget enrichment). Persisted so the client's NEXT request is
/// denied/redirected immediately without any cross-service round trip.
/// </summary>
public sealed record ApiAccessRecallEntry(
    Guid? RuleId,
    ApiAccessRestrictionActionDto Action,
    string? Reason,
    DateTimeOffset CreatedAtUtc);
