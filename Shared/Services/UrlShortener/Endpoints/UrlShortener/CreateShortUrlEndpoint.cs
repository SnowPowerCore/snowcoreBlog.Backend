using FastEndpoints;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Steps;
using snowcoreBlog.PublicApi.Extensions;
using System.Net;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http.Json;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.UrlShortener.Endpoints.UrlShortener;

public class CreateShortUrlRequest
{
    public string OriginalUrl { get; set; } = string.Empty;
    public int? MaxClicks { get; set; }
    public int? WindowSeconds { get; set; }
    public string? CustomCode { get; set; }
}

public class CreateShortUrlResult
{
    public string Code { get; set; } = string.Empty;
    public string ShortUrl { get; set; } = string.Empty;
}

public class CreateShortUrlEndpoint : Endpoint<CreateShortUrlRequest, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }
    
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
