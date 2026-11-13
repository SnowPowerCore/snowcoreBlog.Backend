using MaybeResults;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.Extensions;
using snowcoreBlog.Backend.UrlShortener.Options;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.UrlShortener.Steps.Create;

public sealed class BuildCreateShortUrlResultStep(IOptions<UrlShortenerOptions> opts) : IStep<CreateShortUrlDelegate, CreateShortUrlContext, IMaybe<CreateShortUrlResult>>
{
    public Task<IMaybe<CreateShortUrlResult>> InvokeAsync(CreateShortUrlContext context, CreateShortUrlDelegate next, CancellationToken token = default)
    {
        var mapping = context.Mapping!;
        var baseUrl = opts.Value.BaseUrl?.TrimEnd('/') ?? "https://localhost";

        var result = mapping.ToCreateShortUrlResult();
        result.ShortUrl = $"{baseUrl}/r/{mapping.Code}";

        return Task.FromResult(Maybe.Create(result));
    }
}