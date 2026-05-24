using System.Net;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.Models;

namespace snowcoreBlog.Backend.UrlShortener.Steps;

public sealed class ValidateOriginalUrlStep : IStep<CreateShortUrlDelegate, CreateShortUrlContext, IMaybe<CreateShortUrlOperationResult>>
{
    public Task<IMaybe<CreateShortUrlOperationResult>> InvokeAsync(CreateShortUrlContext context, CreateShortUrlDelegate next, CancellationToken token = default)
    {
        if (context is null || context.Request is null || string.IsNullOrWhiteSpace(context.Request.OriginalUrl))
        {
            return Task.FromResult(Maybe.Create(new CreateShortUrlOperationResult
            {
                HttpStatusCode = (int)HttpStatusCode.BadRequest,
                Response = ErrorResponseUtilities.ApiResponseWithErrors(["OriginalUrl is required"], (int)HttpStatusCode.BadRequest)
            }));
        }

        if (!Uri.TryCreate(context.Request.OriginalUrl, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return Task.FromResult(Maybe.Create(new CreateShortUrlOperationResult
            {
                HttpStatusCode = (int)HttpStatusCode.BadRequest,
                Response = ErrorResponseUtilities.ApiResponseWithErrors(["OriginalUrl must be a valid http(s) URL"], (int)HttpStatusCode.BadRequest)
            }));
        }

        return next(context, token);
    }
}