using System.Text;
using System.Text.Json;
using FastEndpoints;
using Ixnas.AltchaNet;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.Infrastructure.Processors;

public class AltchaVerificationProcessor(AltchaService altcha) : IGlobalPreProcessor
{
    private readonly static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    }.SetJsonSerializationContext();

    private readonly AltchaService _altcha = altcha;

    public async Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
    {
        var errorMessage = $"The [{HeaderKeyConstants.CaptchaHeader}] header has to be set!";
        if (context.HttpContext.Request.Headers.TryGetValue(HeaderKeyConstants.CaptchaHeader, out var captchaSolutionBase64))
        {
            var captchaSolutionJson = Encoding.UTF8.GetString(Convert.FromBase64String(captchaSolutionBase64));
            var captchaSolution = JsonSerializer.Deserialize<AltchaResponse>(captchaSolutionJson, _serializerOptions);
            var validationResult = await _altcha.Validate(captchaSolution, ct);
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