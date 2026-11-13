using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.ErrorResults;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.UrlShortener.Steps.Create;

public sealed class ValidateOriginalUrlStep : IStep<CreateShortUrlDelegate, CreateShortUrlContext, IMaybe<CreateShortUrlResult>>
{
    public Task<IMaybe<CreateShortUrlResult>> InvokeAsync(CreateShortUrlContext context, CreateShortUrlDelegate next, CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(context.Request?.OriginalUrl))
        {
            return Task.FromResult(OriginalUrlRequiredError<CreateShortUrlResult>.Create(
                "OriginalUrl is required."));
        }

        if (!Uri.TryCreate(context.Request.OriginalUrl, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return Task.FromResult(OriginalUrlInvalidError<CreateShortUrlResult>.Create(
                "OriginalUrl must be a valid http(s) URL."));
        }

        return next(context, token);
    }
}