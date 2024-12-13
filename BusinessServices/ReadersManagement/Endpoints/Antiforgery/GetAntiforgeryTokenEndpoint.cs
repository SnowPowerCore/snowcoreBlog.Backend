using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Results;
using snowcoreBlog.PublicApi.Extensions;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.Antiforgery;

public class GetAntiforgeryTokenEndpoint : EndpointWithoutRequest
{
    public IAntiforgery _antiforgery;

    public IOptions<JsonOptions> JsonOptions { get; set; }

    public override void Configure()
    {
        Get("antiForgeryToken");
    }

    public GetAntiforgeryTokenEndpoint(IAntiforgery antiforgery)
    {
        _antiforgery = antiforgery;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var tokenSet = _antiforgery.GetAndStoreTokens(HttpContext);
        var result = Result.Success(tokenSet);
        await SendAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}