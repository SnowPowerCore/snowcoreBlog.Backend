using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Gateway.Middleware.Middleware;

/// <summary>
/// Gateway-level API access restriction enforcement.
///
/// Hot path is purely local: the request is matched against the in-memory compiled
/// restrictions snapshot (refreshed out-of-band by
/// <c>ApiAccessRestrictionsSnapshotRefreshHostedService</c> and the change-event
/// consumer) - there is NO per-request microservice-to-microservice round trip.
///
/// Deny semantics are content-negotiated: browser navigation (Accept: text/html) is
/// redirected to the "unable to access" frontend page, API/XHR callers receive the
/// template StatusCode/ContentType/BodyJson. RequireCaptcha is always answered with
/// 403 plus an action header so the frontend can drive its captcha flow.
///
/// Heavy signals implement <see cref="IApiAccessBackgroundEnrichment"/> and run
/// fire-and-forget after the response completed; a resulting denial is persisted via
/// <see cref="IApiAccessRecallStore"/> and enforced on the client's NEXT request.
///
/// Failure policy: ANY error inside the restriction process (recall lookup, snapshot
/// access, evaluation, request building, deny application) means "no restriction
/// applied" - partially applied restriction artifacts are cleared and the request is
/// let through (see <see cref="ApiAccessRestrictionMiddlewareOptions.FailOpen"/>).
/// The only exception is a response that has already started, where a let-through is
/// physically impossible.
/// </summary>
public sealed class ApiAccessRestrictionMiddleware(RequestDelegate next,
                                                   IOptions<ApiAccessRestrictionMiddlewareOptions> options,
                                                   IApiAccessRestrictionsSnapshotStore snapshotStore,
                                                   ILocalApiAccessRestrictionsEvaluator evaluator,
                                                   IApiAccessRecallStore recallStore,
                                                   ILogger<ApiAccessRestrictionMiddleware> logger,
                                                   IEnumerable<IApiAccessBackgroundEnrichment> backgroundEnrichments)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var optionsValue = options.Value;
        var path = context.Request.Path.Value ?? "/";

        if (IsExcluded(path, optionsValue))
        {
            await next(context);
            return;
        }

        // The entire restriction process lives inside this guard: any failure results
        // in an unrestricted let-through. The closure flag lets the fail-open fallback
        // cancel a fire-and-forget background evaluation that may already be scheduled.
        var backgroundEvaluationAllowed = true;
        try
        {
            var clientId = ResolveClientId(context);

            // 0) Post-response restriction decisions (fire-and-forget evaluations) take
            //    precedence and are served from the shared recall store - still no round trip.
            ApiAccessRecallEntry? recall = null;
            try
            {
                recall = await recallStore.TryGetAsync(clientId, context.RequestAborted);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested) { }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "ApiAccessRestrictions recall lookup failed for client '{ClientId}'.", clientId);
            }

            if (recall is not null)
            {
                await DenyAsync(context, ToDecision(recall), optionsValue);
                return;
            }

            // 1) Local in-memory evaluation against the compiled snapshot.
            var snapshot = snapshotStore.CurrentCompiled;
            if (snapshot is null)
            {
                if (optionsValue.FailOpen)
                {
                    await next(context);
                    return;
                }

                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                return;
            }

            var request = BuildCheckRequest(context, path, optionsValue);
            var decision = evaluator.Evaluate(snapshot, request);

            if (!decision.IsAllowed)
            {
                await DenyAsync(context, decision, optionsValue);
                return;
            }

            if (decision.Action == ApiAccessRestrictionActionDto.AllowWithWarning && !string.IsNullOrWhiteSpace(decision.Reason))
            {
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers[WarningHeader] = decision.Reason!;
                    return Task.CompletedTask;
                });
            }

            ScheduleBackgroundEvaluation(context, request, clientId, optionsValue, backgroundEvaluationAllowed);

            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // Client is gone; there is nobody left to let through.
            throw;
        }
        catch (Exception ex)
        {
            backgroundEvaluationAllowed = false;

            if (context.Response.HasStarted)
            {
                logger.LogError(ex, "ApiAccessRestrictions failed after the response started for '{Path}'; cannot unwind to a let-through.", path);
                return;
            }

            if (!optionsValue.FailOpen)
            {
                logger.LogError(ex, "ApiAccessRestrictions failed with FailOpen disabled for '{Path}'; returning 503.", path);
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                return;
            }

            // Treat the error as "no restriction": wipe any partially applied restriction
            // artifacts (status code, Location / X-Access-Restriction-* headers, pending
            // OnStarting callback) and let the request through the pipeline untouched.
            logger.LogWarning(ex, "ApiAccessRestrictions failed for '{Path}'; failing open and letting the request through.", path);
            context.Response.Clear();
            await next(context);
        }
    }

    private const string WarningHeader = "X-Access-Restriction-Warning";
    private const string CaptchaActionHeader = "X-Access-Restriction-Action";

    private static bool IsExcluded(string path, ApiAccessRestrictionMiddlewareOptions optionsValue) =>
        optionsValue.ExcludedPathPrefixes.Any(prefix =>
            path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));

    private async Task DenyAsync(HttpContext context, CheckApiAccessResponseDto decision, ApiAccessRestrictionMiddlewareOptions optionsValue)
    {
        if (decision.Action == ApiAccessRestrictionActionDto.RequireCaptcha)
        {
            // Captcha flow is driven by the frontend against the JSON payload;
            // no redirect even for browser navigation.
            context.Response.Headers[CaptchaActionHeader] = "require-captcha";
        }
        else if (WantsHtml(context))
        {
            context.Response.Redirect(BuildDeniedPageUrl(optionsValue.DeniedPagePath, decision));
            return;
        }

        context.Response.StatusCode = decision.StatusCode ?? StatusCodes.Status403Forbidden;
        context.Response.ContentType = decision.ContentType ?? "application/json";

        if (!string.IsNullOrEmpty(decision.BodyJson))
        {
            await context.Response.WriteAsync(decision.BodyJson!);
        }
    }

    private static bool WantsHtml(HttpContext context) =>
        context.Request.Headers.Accept.ToString().Contains("text/html", StringComparison.OrdinalIgnoreCase);

    private static string BuildDeniedPageUrl(string deniedPagePath, CheckApiAccessResponseDto decision)
    {
        var query = new Dictionary<string, string?>();
        if (!string.IsNullOrWhiteSpace(decision.Reason))
            query["reason"] = decision.Reason;
        if (decision.MatchedRuleId is { } ruleId)
            query["ruleId"] = ruleId.ToString();

        return deniedPagePath + QueryString.Create(query).Value;
    }

    private static CheckApiAccessRequestDto BuildCheckRequest(HttpContext context, string path, ApiAccessRestrictionMiddlewareOptions optionsValue)
    {
        var country = context.Request.Headers[optionsValue.CountryHeaderName].ToString();
        var tags = ResolveClientTags(context);

        return new CheckApiAccessRequestDto
        {
            Path = path,
            Method = context.Request.Method,
            Ip = context.Connection.RemoteIpAddress?.ToString(),
            CountryCode = string.IsNullOrWhiteSpace(country) ? null : country,
            Tags = tags.Count > 0 ? tags : null
        };
    }

    private static List<string> ResolveClientTags(HttpContext context)
    {
        List<string>? tags = null;
        if (context.User.Identity?.IsAuthenticated == true)
        {
            foreach (var claim in context.User.Claims)
            {
                if (claim.Type == "readerAccount" && string.Equals(claim.Value, bool.TrueString, StringComparison.OrdinalIgnoreCase))
                {
                    (tags ??= []).Add("reader");
                }
                else if (claim.Type is ClaimTypes.Role or "role")
                {
                    (tags ??= []).Add(claim.Value);
                }
            }
        }

        return tags ?? [];
    }

    private static string ResolveClientId(HttpContext context)
    {
        var subject = context.User.FindFirst("sub")?.Value ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrWhiteSpace(subject))
            return $"u:{subject}";

        var ip = context.Connection.RemoteIpAddress?.ToString();
        return string.IsNullOrWhiteSpace(ip) ? "anonymous" : $"ip:{ip}";
    }

    private static CheckApiAccessResponseDto ToDecision(ApiAccessRecallEntry recall) => new()
    {
        IsAllowed = false,
        MatchedRuleId = recall.RuleId,
        Action = recall.Action,
        Reason = recall.Reason ?? "Access restricted by policy.",
        StatusCode = StatusCodes.Status403Forbidden,
        ContentType = "application/json"
    };

    private void ScheduleBackgroundEvaluation(HttpContext context, CheckApiAccessRequestDto request, string clientId, ApiAccessRestrictionMiddlewareOptions optionsValue, bool backgroundEvaluationAllowed)
    {
        var enrichments = backgroundEnrichments as IApiAccessBackgroundEnrichment[] ?? [.. backgroundEnrichments.OrderBy(static e => e.Order)];
        if (enrichments.Length == 0)
            return;

        context.Response.OnCompleted(async () =>
        {
            // The fail-open fallback may have cancelled this evaluation after a
            // restriction-process error (captured variable, not a value copy).
            if (!backgroundEvaluationAllowed)
                return;

            foreach (var enrichment in enrichments)
            {
                CheckApiAccessResponseDto? backgroundDecision;
                try
                {
                    backgroundDecision = await enrichment.EvaluateAfterResponseAsync(request, context, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Background access evaluation '{Enrichment}' failed.", enrichment.GetType().Name);
                    continue;
                }

                if (backgroundDecision is not { IsAllowed: false })
                    continue;

                try
                {
                    await recallStore.SetAsync(
                        clientId,
                        new ApiAccessRecallEntry(
                            backgroundDecision.MatchedRuleId,
                            backgroundDecision.Action ?? ApiAccessRestrictionActionDto.Block,
                            backgroundDecision.Reason,
                            DateTimeOffset.UtcNow),
                        optionsValue.RecallTimeToLive,
                        CancellationToken.None);

                    logger.LogInformation("Background access evaluation restricted client '{ClientId}' until recall expiry.", clientId);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to persist access recall for client '{ClientId}'.", clientId);
                }
            }
        });
    }
}
