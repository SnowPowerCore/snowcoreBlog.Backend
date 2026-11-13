using System.Net;
using FastEndpoints;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.Steps.Resolve;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.UrlShortener.Endpoints.UrlShortener;

public class ResolveShortUrlEndpoint : EndpointWithoutRequest
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

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
        var context = new ResolveShortUrlContext(Route<string>("code") ?? string.Empty);

        var result = await ResolveShortUrl(context, ct);

        // Successful resolves redirect (302) to the target URL; failures are
        // mapped to the standard envelope at endpoint level.
        if (result is Some<ResolveShortUrlResultDto> resolved
            && !string.IsNullOrWhiteSpace(resolved.Value.RedirectUrl))
        {
            HttpContext.Response.Redirect(resolved.Value.RedirectUrl);
            return;
        }

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
