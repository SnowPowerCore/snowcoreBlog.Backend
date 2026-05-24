using System.Net;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.UrlShortener.Models;
using snowcoreBlog.Backend.Infrastructure.Utilities;

namespace snowcoreBlog.Backend.UrlShortener.Steps;

public sealed class LoadMappingStep(IUrlMappingRepository repo) : IStep<ResolveShortUrlDelegate, ResolveShortUrlContext, IMaybe<ResolveShortUrlOperationResult>>
{
    public async Task<IMaybe<ResolveShortUrlOperationResult>> InvokeAsync(ResolveShortUrlContext context, ResolveShortUrlDelegate next, CancellationToken token = default)
    {
        var code = context.Code ?? string.Empty;
        if (string.IsNullOrWhiteSpace(code))
        {
            return Maybe.Create(new ResolveShortUrlOperationResult
            {
                HttpStatusCode = (int)HttpStatusCode.BadRequest,
                Response = ErrorResponseUtilities.ApiResponseWithErrors(["Missing code"], (int)HttpStatusCode.BadRequest)
            });
        }

        var mapping = await repo.GetByCodeAsync(code, token);
        if (mapping is null)
        {
            return Maybe.Create(new ResolveShortUrlOperationResult
            {
                HttpStatusCode = (int)HttpStatusCode.NotFound,
                Response = ErrorResponseUtilities.ApiResponseWithErrors(["Not found"], (int)HttpStatusCode.NotFound)
            });
        }

        context.Mapping = mapping;
        return await next(context, token);
    }
}