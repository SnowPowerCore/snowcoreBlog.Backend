namespace snowcoreBlog.Backend.Gateway.Middleware.Middleware;

/// <summary>
/// Configuration for <see cref="ApiAccessRestrictionMiddleware"/>, bound from the
/// "ApiAccessRestriction" configuration section of the gateway host.
/// </summary>
public sealed class ApiAccessRestrictionMiddlewareOptions
{
    public const string SectionName = "ApiAccessRestriction";

    /// <summary>Header carrying the client's ISO country code (trusted edge header). Absent = region rules skipped.</summary>
    public string CountryHeaderName { get; set; } = "X-Country-Code";

    /// <summary>Frontend page browser clients are redirected to when blocked. The frontend is served under /app.</summary>
    public string DeniedPagePath { get; set; } = "/app/access-denied";

    /// <summary>Path prefixes never restricted (health probes, the restrictions API itself, the denied page - avoids redirect loops).</summary>
    public IReadOnlyList<string> ExcludedPathPrefixes { get; set; } = ["/health", "/alive", "/api/access", "/app/access-denied"];

    /// <summary>
    /// When true (default), ANY error in the restriction process (recall lookup,
    /// snapshot access, evaluation, request building, deny application) results in
    /// "no restriction applied" - the request is let through (availability over
    /// enforcement). When false, errors answer with 503 instead.
    /// </summary>
    public bool FailOpen { get; set; } = true;

    /// <summary>How long a fire-and-forget (post-response) restriction decision stays active for the client.</summary>
    public TimeSpan RecallTimeToLive { get; set; } = TimeSpan.FromMinutes(15);
}
