using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.Steps.Create;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.UrlShortener.Endpoints.UrlShortener;

public class CreateShortUrlEndpoint : Endpoint<CreateShortUrlRequest, ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ValidateOriginalUrlStep),
        typeof(GenerateCodeStep),
        typeof(PersistUrlMappingStep),
        typeof(BuildCreateShortUrlResultStep)
    ])]
    protected CreateShortUrlDelegate CreateShortUrl { get; } = default!;

    public override void Configure()
    {
        Post("create");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
    }

    public override async Task HandleAsync(CreateShortUrlRequest req, CancellationToken ct)
    {
        var context = new CreateShortUrlContext(req);
        
        var result = await CreateShortUrl(context, ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
