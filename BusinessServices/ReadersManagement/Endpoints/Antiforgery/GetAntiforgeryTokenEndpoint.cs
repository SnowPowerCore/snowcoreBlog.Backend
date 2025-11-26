using System.Net;
using System.Net.Mime;
using FastEndpoints;
using MaybeResults;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

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
        Description(b => b
            .WithTags(ApiTagConstants.Tokens)
            .Produces<ApiResponseForOpenApi<AntiforgeryResultDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.InternalServerError, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public GetAntiforgeryTokenEndpoint(IAntiforgery antiforgery)
    {
        _antiforgery = antiforgery;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // Delete any existing antiforgery cookie to force generation of a fresh token
        // This ensures we always get a new token that matches the current authentication state
        var existingCookies = HttpContext.Request.Cookies
            .Where(c => c.Key.StartsWith(".AspNetCore.Antiforgery", StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var cookie in existingCookies)
        {
            HttpContext.Response.Cookies.Delete(cookie.Key, new CookieOptions
            {
                Path = "/",
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            });
        }

        var tokenSet = _antiforgery.GetAndStoreTokens(HttpContext);
        var result = Maybe.Create(new AntiforgeryResultDto(tokenSet.RequestToken, tokenSet.HeaderName));
        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}