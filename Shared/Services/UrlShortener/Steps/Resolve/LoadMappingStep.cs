using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Core.Entities.UrlShortener;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.ErrorResults;
using snowcoreBlog.Backend.UrlShortener.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.UrlShortener.Steps.Resolve;

public sealed class LoadMappingStep(IUrlMappingRepository repo) : IStep<ResolveShortUrlDelegate, ResolveShortUrlContext, IMaybe<ResolveShortUrlResultDto>>
{
    public async Task<IMaybe<ResolveShortUrlResultDto>> InvokeAsync(ResolveShortUrlContext context, ResolveShortUrlDelegate next, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(context.Code))
        {
            return ShortUrlCodeMissingError<ResolveShortUrlResultDto>.Create(
                "Short URL code is required.");
        }

        var mapping = await repo.GetByCodeAsync(context.Code, token);
        if (mapping is default(UrlMappingEntity))
        {
            return ShortUrlNotFoundError<ResolveShortUrlResultDto>.Create(
                $"Short URL '{ context.Code }' was not found.");
        }

        context.Mapping = mapping;
        return await next(context, token);
    }
}