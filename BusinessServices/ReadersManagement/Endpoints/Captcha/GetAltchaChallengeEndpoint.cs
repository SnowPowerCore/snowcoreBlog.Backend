using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Ixnas.AltchaNet;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.Captcha;

public class GetAltchaChallengeEndpoint(AltchaService altcha) : EndpointWithoutRequest<AltchaChallenge?>
{
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

    public override Task HandleAsync(CancellationToken ct) =>
        Send.ResponseAsync(altcha.Generate(), (int)HttpStatusCode.OK, ct);
}