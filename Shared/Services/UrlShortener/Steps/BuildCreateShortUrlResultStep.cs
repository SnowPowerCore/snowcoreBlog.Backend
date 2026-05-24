using System.Net;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.Models;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.Backend.UrlShortener.Options;

namespace snowcoreBlog.Backend.UrlShortener.Steps;

public sealed class BuildCreateShortUrlResultStep(IOptions<UrlShortenerOptions> opts) : IStep<CreateShortUrlDelegate, CreateShortUrlContext, IMaybe<CreateShortUrlOperationResult>>
{
    private readonly UrlShortenerOptions _opts = opts.Value;

    public Task<IMaybe<CreateShortUrlOperationResult>> InvokeAsync(CreateShortUrlContext context, CreateShortUrlDelegate next, CancellationToken token = default)
    {
        var mapping = context.Mapping!;
        var baseUrl = _opts.BaseUrl?.TrimEnd('/') ?? "https://localhost";

        var resultObj = new { mapping.Code, ShortUrl = $"{baseUrl}/r/{mapping.Code}" };

        var op = new CreateShortUrlOperationResult
        {
            HttpStatusCode = (int)HttpStatusCode.OK,
            Response = new ApiResponse(resultObj.ToJsonDocument(null), 1, (int)HttpStatusCode.OK, Array.Empty<string>())
        };

        return Task.FromResult(Maybe.Create(op));
    }
}