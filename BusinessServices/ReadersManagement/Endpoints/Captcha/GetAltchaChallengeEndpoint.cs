using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Ixnas.AltchaNet;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.Captcha;

public class GetAltchaChallengeEndpoint : EndpointWithoutRequest<AltchaChallenge?>
{
    private readonly AltchaService _altcha;

    public IOptions<JsonOptions> JsonOptions { get; set; }

    public override void Configure()
    {
        Get("captcha/challenge");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.Tokens)
            .Produces<AltchaChallenge>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public GetAltchaChallengeEndpoint(AltchaService altcha)
    {
        _altcha = altcha;
    }

    public override Task HandleAsync(CancellationToken ct) =>
        SendAsync(_altcha.Generate(), (int)HttpStatusCode.OK, ct);
}