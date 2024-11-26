using System.Net;
using FastEndpoints;
using Ixnas.AltchaNet;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Results;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.Captcha;

public class GetAltchaChallengeEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    private readonly AltchaService _altcha;

    public IOptions<JsonOptions> JsonOptions { get; set; }

    public override void Configure()
    {
        Get("captcha/challenge");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
    }

    public GetAltchaChallengeEndpoint(AltchaService altcha)
    {
        _altcha = altcha;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = Result.Success(_altcha.Generate());

        await SendAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}