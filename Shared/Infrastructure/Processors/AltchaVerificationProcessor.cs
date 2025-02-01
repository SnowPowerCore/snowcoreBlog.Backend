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
        var errorMessage = $"The [{HeaderKeyConstants.AltchaCaptchaHeader}] header has to be set!";
        if (context.HttpContext.Request.Headers.TryGetValue(HeaderKeyConstants.AltchaCaptchaHeader, out var captchaSolution))
        {
            var validationResult = await _altcha.Validate(captchaSolution);
            if (validationResult?.IsValid ?? false)
            {
                return;
            }
            else
            {
                errorMessage = validationResult?.ValidationError.Message;
            }
        }
        context.ValidationFailures.Add(new("AltchaValidationError", errorMessage));
        await context.HttpContext.Response.SendErrorsAsync(context.ValidationFailures, cancellation: ct);
    }
}