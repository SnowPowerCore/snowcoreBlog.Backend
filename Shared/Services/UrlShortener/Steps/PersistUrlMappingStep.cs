using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.Core.Entities;
using snowcoreBlog.Backend.UrlShortener.Models;
using snowcoreBlog.Backend.UrlShortener.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.UrlShortener.Options;

namespace snowcoreBlog.Backend.UrlShortener.Steps;

public sealed class PersistUrlMappingStep(IUrlMappingRepository repo, IOptions<UrlShortenerOptions> opts) : IStep<CreateShortUrlDelegate, CreateShortUrlContext, IMaybe<CreateShortUrlOperationResult>>
{
    private readonly UrlShortenerOptions _opts = opts.Value;

    public async Task<IMaybe<CreateShortUrlOperationResult>> InvokeAsync(CreateShortUrlContext context, CreateShortUrlDelegate next, CancellationToken token = default)
    {
        var code = context.Code ?? string.Empty;
        var req = context.Request;

        var entity = new UrlMappingEntity
        {
            Id = Guid.NewGuid(),
            Code = code,
            OriginalUrl = req.OriginalUrl,
            CreatedAt = DateTime.UtcNow,
            MaxClicksPerWindow = req.MaxClicks ?? (_opts.DefaultMaxClicksPerWindow == 0 ? default : _opts.DefaultMaxClicksPerWindow),
            WindowDurationSeconds = req.WindowSeconds ?? (_opts.DefaultWindowSeconds == 0 ? default : _opts.DefaultWindowSeconds),
            IsActive = true
        };

        var persisted = await repo.AddOrUpdateAsync(entity, entity.Id, true, token);
        context.Mapping = persisted;

        return await next(context, token);
    }
}