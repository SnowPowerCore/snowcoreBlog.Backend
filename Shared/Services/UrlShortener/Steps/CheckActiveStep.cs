using System.Net;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.Models;
using snowcoreBlog.Backend.Infrastructure.Utilities;

namespace snowcoreBlog.Backend.UrlShortener.Steps;

public sealed class CheckActiveStep : IStep<ResolveShortUrlDelegate, ResolveShortUrlContext, IMaybe<ResolveShortUrlOperationResult>>
{
    public Task<IMaybe<ResolveShortUrlOperationResult>> InvokeAsync(ResolveShortUrlContext context, ResolveShortUrlDelegate next, CancellationToken token = default)
    {
        var mapping = context.Mapping;
        if (mapping is default(Core.Entities.UrlMappingEntity) || !mapping.IsActive)
        {
            return Task.FromResult(Maybe.Create(new ResolveShortUrlOperationResult
            {
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Response = ErrorResponseUtilities.ApiResponseWithErrors(["Not found"], (int)HttpStatusCode.NotFound)
            }));
        }

        return next(context, token);
    }
}