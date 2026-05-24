using System.Net;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.Models;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.UrlShortener.Steps;

public sealed class BuildResolveShortUrlResultStep : IStep<ResolveShortUrlDelegate, ResolveShortUrlContext, IMaybe<ResolveShortUrlOperationResult>>
{
    public Task<IMaybe<ResolveShortUrlOperationResult>> InvokeAsync(ResolveShortUrlContext context, ResolveShortUrlDelegate next, CancellationToken token = default)
    {
        var mapping = context.Mapping!;

        var op = new ResolveShortUrlOperationResult
        {
            HttpStatusCode = (int)HttpStatusCode.Found,
            RedirectUrl = mapping.OriginalUrl,
            Response = new ApiResponse(default, 0, (int)HttpStatusCode.Found, [])
        };

        return Task.FromResult(Maybe.Create(op));
    }
}