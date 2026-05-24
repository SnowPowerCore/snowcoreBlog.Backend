using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.Core.Entities;
using snowcoreBlog.Backend.UrlShortener.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.UrlShortener.Models;

namespace snowcoreBlog.Backend.UrlShortener.Steps;

public sealed class RecordClickStep(IUrlMappingRepository repo) : IStep<ResolveShortUrlDelegate, ResolveShortUrlContext, IMaybe<ResolveShortUrlOperationResult>>
{
    public async Task<IMaybe<ResolveShortUrlOperationResult>> InvokeAsync(ResolveShortUrlContext context, ResolveShortUrlDelegate next, CancellationToken token = default)
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