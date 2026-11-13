using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.ErrorResults;
using snowcoreBlog.Backend.UrlShortener.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.UrlShortener.Steps.Resolve;

public sealed class CheckRateLimitStep(IUrlMappingRepository repo) : IStep<ResolveShortUrlDelegate, ResolveShortUrlContext, IMaybe<ResolveShortUrlResultDto>>
{
    public async Task<IMaybe<ResolveShortUrlResultDto>> InvokeAsync(ResolveShortUrlContext context, ResolveShortUrlDelegate next, CancellationToken token = default)
    {
        var mapping = context.Mapping!;
        if (mapping.MaxClicksPerWindow.HasValue && mapping.WindowDurationSeconds.HasValue && mapping.MaxClicksPerWindow.Value > 0)
        {
            var since = DateTime.UtcNow.AddSeconds(-mapping.WindowDurationSeconds.Value);
            var count = await repo.CountClicksInWindowAsync(mapping.Id, since, token);
            if ((long)count >= mapping.MaxClicksPerWindow.Value)
            {
                return ShortUrlRateLimitExceededError<ResolveShortUrlResultDto>.Create(
                    $"Short URL '{ mapping.Code }' has exceeded its click rate limit.");
            }
        }

        return await next(context, token);
    }
}