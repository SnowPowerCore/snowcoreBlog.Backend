using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Ixnas.AltchaNet;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.Backend.Infrastructure;

namespace snowcoreBlog.Backend.Articles.Endpoints.Captcha;

public class GetAltchaChallengeEndpoint : EndpointWithoutRequest<AltchaChallenge?>
{
    private readonly AltchaService _altcha;

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
