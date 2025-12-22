using System.Buffers;
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
            var base64 = captchaSolutionBase64.ToString();
            if (!string.IsNullOrWhiteSpace(base64))
            {
                // base64 max decoded length ~= (len/4)*3 (+ a little slack)
                var maxDecodedLength = (base64.Length / 4) * 3 + 3;
                byte[]? rented = null;
                try
                {
                    Span<byte> decoded = maxDecodedLength <= 4096
                        ? stackalloc byte[maxDecodedLength]
                        : (rented = ArrayPool<byte>.Shared.Rent(maxDecodedLength));

                    if (rented is not null)
                        decoded = decoded.Slice(0, maxDecodedLength);

                    if (Convert.TryFromBase64String(base64, decoded, out var bytesWritten))
                    {
                        var captchaSolutionJson = Encoding.UTF8.GetString(decoded.Slice(0, bytesWritten));
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
                }
                finally
                {
                    if (rented is not null)
                        ArrayPool<byte>.Shared.Return(rented, clearArray: true);
                }
            }
        }
        context.ValidationFailures.Add(new("AltchaValidationError", errorMessage));
        await context.HttpContext.Response.SendErrorsAsync(context.ValidationFailures, cancellation: ct);
    }
}