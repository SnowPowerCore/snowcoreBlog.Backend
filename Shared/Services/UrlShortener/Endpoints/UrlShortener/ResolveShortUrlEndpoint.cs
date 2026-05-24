using System.Net;
using MaybeResults;
using FastEndpoints;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Models;
using snowcoreBlog.Backend.UrlShortener.Steps;
using snowcoreBlog.Backend.Infrastructure.Utilities;

namespace snowcoreBlog.Backend.UrlShortener.Endpoints.UrlShortener;

public class ResolveShortUrlEndpoint : EndpointWithoutRequest
{
    [StepifiedProcess(Steps = [
        typeof(LoadMappingStep),
        typeof(CheckActiveStep),
        typeof(CheckRateLimitStep),
        typeof(RecordClickStep),
        typeof(BuildResolveShortUrlResultStep)
    ])]
    protected ResolveShortUrlDelegate ResolveShortUrl { get; } = default!;

    public override void Configure()
    {
        Get("r/{code}");
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var code = HttpContext.Request.RouteValues["code"]?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(code))
        {
            await Send.StatusCodeAsync((int)HttpStatusCode.BadRequest, ct);
            return;
        }

        var context = new ResolveShortUrlContext(code);
        var maybeResult = await ResolveShortUrl(context, ct);

        var op = maybeResult is Some<ResolveShortUrlOperationResult> s
            ? s.Value
            : new ResolveShortUrlOperationResult
            {
                HttpStatusCode = 500,
                Response = ErrorResponseUtilities.ApiResponseWithErrors(["Unknown error"], 500)
            };

        if (!string.IsNullOrWhiteSpace(op.RedirectUrl))
        {
            HttpContext.Response.Redirect(op.RedirectUrl!);
            return;
        }

        await Send.ResponseAsync(op.Response, op.HttpStatusCode, ct);
    }
}
