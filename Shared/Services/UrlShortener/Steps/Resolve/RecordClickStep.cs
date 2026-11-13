using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Core.Entities.UrlShortener;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.UrlShortener.Steps.Resolve;

public sealed class RecordClickStep(IUrlMappingRepository repo) : IStep<ResolveShortUrlDelegate, ResolveShortUrlContext, IMaybe<ResolveShortUrlResultDto>>
{
    public async Task<IMaybe<ResolveShortUrlResultDto>> InvokeAsync(ResolveShortUrlContext context, ResolveShortUrlDelegate next, CancellationToken token = default)
    {
        var mapping = context.Mapping!;

        var click = new ClickEventEntity
        {
            Id = Guid.NewGuid(),
            UrlMappingId = mapping.Id,
            ClickedAt = DateTime.UtcNow
        };

        await repo.RecordClickAsync(click, token);

        return await next(context, token);
    }
}