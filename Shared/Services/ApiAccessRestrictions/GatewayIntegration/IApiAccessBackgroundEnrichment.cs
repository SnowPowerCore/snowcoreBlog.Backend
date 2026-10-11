using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;

/// <summary>
/// Seam for heavier access signals (GeoIP enrichment, per-client velocity counters, ...)
/// that run fire-and-forget AFTER the endpoint/response completed. A returned denial is
/// persisted as a recall entry so the client's NEXT request is denied/redirected by the
/// gateway middleware - the current response is never blocked retroactively.
/// </summary>
public interface IApiAccessBackgroundEnrichment
{
    /// <summary>Lower runs first.</summary>
    int Order { get; }

    /// <returns>A denial (<see cref="CheckApiAccessResponseDto.IsAllowed"/> false) to restrict the client, or null to allow.</returns>
    Task<CheckApiAccessResponseDto?> EvaluateAfterResponseAsync(CheckApiAccessRequestDto request, HttpContext httpContext, CancellationToken cancellationToken);
}
