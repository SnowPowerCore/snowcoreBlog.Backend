using System.Net;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.UrlShortener.Models;
using snowcoreBlog.Backend.Infrastructure.Utilities;

namespace snowcoreBlog.Backend.UrlShortener.Steps;

public sealed class CheckRateLimitStep(IUrlMappingRepository repo) : IStep<ResolveShortUrlDelegate, ResolveShortUrlContext, IMaybe<ResolveShortUrlOperationResult>>
{
    public async Task<IMaybe<ResolveShortUrlOperationResult>> InvokeAsync(ResolveShortUrlContext context, ResolveShortUrlDelegate next, CancellationToken token = default)
    {
        var mapping = context.Mapping!;
        if (mapping.MaxClicksPerWindow.HasValue && mapping.WindowDurationSeconds.HasValue && mapping.MaxClicksPerWindow.Value > 0)
        {
            var since = DateTime.UtcNow.AddSeconds(-mapping.WindowDurationSeconds.Value);
            var count = await repo.CountClicksInWindowAsync(mapping.Id, since, token);
            if ((long)count >= mapping.MaxClicksPerWindow.Value)
            {
                return Maybe.Create(new ResolveShortUrlOperationResult
                {
                    HttpStatusCode = 429,
                    Response = ErrorResponseUtilities.ApiResponseWithErrors(["Rate limit exceeded"], 429)
                });
            }
        }

        return await next(context, token);
    }
}