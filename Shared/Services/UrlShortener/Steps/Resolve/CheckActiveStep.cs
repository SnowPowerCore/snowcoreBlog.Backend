using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.UrlShortener.Core.Entities.UrlShortener;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.ErrorResults;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.UrlShortener.Steps.Resolve;

public sealed class CheckActiveStep : IStep<ResolveShortUrlDelegate, ResolveShortUrlContext, IMaybe<ResolveShortUrlResultDto>>
{
    public Task<IMaybe<ResolveShortUrlResultDto>> InvokeAsync(ResolveShortUrlContext context, ResolveShortUrlDelegate next, CancellationToken token = default)
    {
        if (context.Mapping is default(UrlMappingEntity) || !context.Mapping.IsActive)
        {
            return Task.FromResult(ShortUrlInactiveError<ResolveShortUrlResultDto>.Create(
                $"Short URL '{ context.Code }' is inactive."));
        }

        return next(context, token);
    }
}