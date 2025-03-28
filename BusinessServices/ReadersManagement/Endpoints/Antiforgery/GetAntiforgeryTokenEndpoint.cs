using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MaybeResults;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.Antiforgery;

public class GetAntiforgeryTokenEndpoint : EndpointWithoutRequest
{
    public IAntiforgery _antiforgery;

    public IOptions<JsonOptions> JsonOptions { get; set; }

    public override void Configure()
    {
        Get("antiforgerytoken");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
        Description(b => b.WithTags(ApiTagConstants.Tokens));
    }

    public GetAntiforgeryTokenEndpoint(IAntiforgery antiforgery)
    {
        _antiforgery = antiforgery;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var tokenSet = _antiforgery.GetAndStoreTokens(HttpContext);
        var result = Maybe.Create(new AntiforgeryResultDto(tokenSet.RequestToken, tokenSet.HeaderName));
        await SendAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}