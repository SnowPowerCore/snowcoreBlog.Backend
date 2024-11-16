using System.Net;
using FastEndpoints;
using Ixnas.AltchaNet;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Results;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.ReaderAccounts;

public class GetAltchaChallengeEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    private readonly AltchaService _altcha;
    private readonly JsonOptions _jsonOptions;

    public override void Configure()
    {
        Post("captcha/challenge");
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
    }

    public GetAltchaChallengeEndpoint(AltchaService altcha,
                                      IOptions<JsonOptions> jsonOptions)
    {
        _altcha = altcha;
        _jsonOptions = jsonOptions.Value;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = Result.Success(_altcha.Generate());

        await SendAsync(
            result?.ToApiResponse(serializerOptions: _jsonOptions.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}