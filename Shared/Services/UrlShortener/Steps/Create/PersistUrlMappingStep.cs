using MaybeResults;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.UrlShortener.Core.Entities.UrlShortener;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.UrlShortener.Options;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.UrlShortener.Steps.Create;

public sealed class PersistUrlMappingStep(IUrlMappingRepository repo, IOptions<UrlShortenerOptions> opts) : IStep<CreateShortUrlDelegate, CreateShortUrlContext, IMaybe<CreateShortUrlResult>>
{
    private readonly UrlShortenerOptions _opts = opts.Value;

    public async Task<IMaybe<CreateShortUrlResult>> InvokeAsync(CreateShortUrlContext context, CreateShortUrlDelegate next, CancellationToken token = default)
    {
        var code = context.Code ?? string.Empty;
        var req = context.Request;

        var entity = new UrlMappingEntity
        {
            Id = Guid.NewGuid(),
            Code = code,
            OriginalUrl = req.OriginalUrl,
            CreatedAt = DateTime.UtcNow,
            MaxClicksPerWindow = req.MaxClicks ?? (_opts.DefaultMaxClicksPerWindow == 0 ? null : _opts.DefaultMaxClicksPerWindow),
            WindowDurationSeconds = req.WindowSeconds ?? (_opts.DefaultWindowSeconds == 0 ? null : _opts.DefaultWindowSeconds),
            IsActive = true
        };

        context.Mapping = (UrlMappingEntity?)await repo.AddOrUpdateAsync(entity, entity.Id, true, token);

        return await next(context, token);
    }
}