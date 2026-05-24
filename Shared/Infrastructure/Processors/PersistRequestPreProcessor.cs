using System.Text;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using snowcoreBlog.Backend.Core.Entities.RequestPersistence;
using snowcoreBlog.Backend.Core.Interfaces.Repositories;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.Infrastructure.Processors;

public class PersistRequestPreProcessor(IRequestPersistenceRepository repository) : IGlobalPreProcessor
{
    private const string ItemsKey_OriginalResponseBody = "__Persist_OriginalResponseBody";
    private const string ItemsKey_ResponseCaptureStream = "__Persist_ResponseCaptureStream";
    private const string ItemsKey_PersistedRequestId = "__PersistedRequestId";

    public async Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
    {
        var http = context.HttpContext;
        if (!http.Request.Headers.TryGetValue(HeaderKeyConstants.PersistHeader, out var persistHeader) || string.IsNullOrWhiteSpace(persistHeader))
            return;

        // Read request body (enable buffering so the endpoint can still consume it)
        http.Request.EnableBuffering();

        string requestBody = string.Empty;
        try
        {
            http.Request.Body.Position = 0;
            using var sr = new StreamReader(http.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            requestBody = await sr.ReadToEndAsync(ct);
            http.Request.Body.Position = 0;
        }
        catch
        {
            // ignore read errors; continue without payload
        }

        // Replace response body with a capture stream so we can inspect the response in the post-processor
        var originalResponseBody = http.Response.Body;
        var captureStream = new MemoryStream();
        http.Response.Body = captureStream;

        http.Items[ItemsKey_OriginalResponseBody] = originalResponseBody;
        http.Items[ItemsKey_ResponseCaptureStream] = captureStream;

        // Create persisted request record
        var idempotencyKey = http.Request.Headers.TryGetValue("Idempotency-Key", out var idKey) ? idKey.ToString() : null;

        var now = DateTimeOffset.UtcNow;

        var entity = new PersistedRequestEntity
        {
            RequestId = Guid.Empty,
            IdempotencyKey = idempotencyKey,
            Status = RequestPersistenceStatus.Persisted,
            RequestPayload = requestBody ?? string.Empty,
            ResponsePayload = null,
            CreatedAt = now,
            UpdatedAt = now,
            RetryCount = 0
        };

        var stored = await repository.CreateAsync(entity, ct);

        // Keep id for post-processor
        http.Items[ItemsKey_PersistedRequestId] = stored.Id;
    }
}