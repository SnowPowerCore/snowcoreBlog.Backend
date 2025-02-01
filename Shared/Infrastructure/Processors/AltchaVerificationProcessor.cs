using FastEndpoints;
using Ixnas.AltchaNet;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.Infrastructure.Processors;

public class AltchaVerificationProcessor : IGlobalPreProcessor
{
    private readonly AltchaService _altcha;

    public AltchaVerificationProcessor(AltchaService altcha)
    {
        _altcha = altcha;
    }

    public async Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
    {
        if (context.HttpContext.Request.Headers.TryGetValue(HeaderKeyConstants.AltchaCaptchaHeader, out var captchaSolution))
        {
            if ((await _altcha.Validate(captchaSolution))?.IsValid ?? false)
            {
                return;
            }
        }
        context.ValidationFailures.Add(
            new("MissingHeaders", $"The [{HeaderKeyConstants.AltchaCaptchaHeader}] header needs to be set!"));
        await context.HttpContext.Response.SendErrorsAsync(context.ValidationFailures, cancellation: ct);
    }
}