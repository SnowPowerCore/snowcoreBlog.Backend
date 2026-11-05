using System.Text;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Core.Entities.RequestPersistence;
using snowcoreBlog.Backend.Core.Interfaces.Repositories;
using snowcoreBlog.Backend.Infrastructure.RequestPersistence;

namespace snowcoreBlog.Backend.Infrastructure.Processors;

/// <summary>
/// Completion side of the durable-claim flow: classifies the outcome, persists the completion
/// marker BEFORE any response bytes reach the client (the marker commit is awaited first), applies
/// the retention policy (tombstone plain first-attempt finishes / keep recovered for audit) and
/// only then flushes the captured response body to the original stream.
/// <para>
/// Streaming (SSE) endpoints skip response capture — only completion status is recorded. On
/// unhandled handler exceptions this processor does not run; the recovery worker takes over after
/// lease expiry.
/// </para>
/// </summary>
/// <remarks>
/// FastEndpoints constructs global processor instances as singletons from the ROOT provider —
/// scoped services are resolved per request via <see cref="IServiceScopeFactory"/>.
/// </remarks>
public class PersistRequestPostProcessor(
    IServiceScopeFactory scopeFactory,
    IOptions<PersistRequestOptions> options,
    ILogger<PersistRequestPostProcessor> logger) : IGlobalPostProcessor
{
    private const string ItemsKeyRequestId = "__PersistRequestId";
    private const string ItemsKeyIsStreaming = "__PersistIsStreaming";
    private const string ItemsKeyCaptureStream = "__PersistCaptureStream";
    private const string ItemsKeyOriginalBody = "__PersistOriginalBody";

    public async Task PostProcessAsync(IPostProcessorContext context, CancellationToken ct)
    {
        RequestResumeScope.Current = null;

        var http = context.HttpContext;
        if (!http.Items.TryGetValue(ItemsKeyRequestId, out var idObj) || idObj is not Guid requestId)
        {
            return;
        }

        var isStreaming = http.Items.TryGetValue(ItemsKeyIsStreaming, out var streamingObj) && streamingObj is true;
        var now = DateTimeOffset.UtcNow;

        await using var scope = scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRequestPersistenceRepository>();

        string? responsePayload = null;
        object? captureObj = null;
        var hasCapture = !isStreaming
                         && http.Items.TryGetValue(ItemsKeyCaptureStream, out captureObj)
                         && captureObj is MemoryStream;
        if (hasCapture)
        {
            var stream = (MemoryStream)captureObj!;
            stream.Position = 0;
            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            responsePayload = await reader.ReadToEndAsync(ct);
            stream.Position = 0;
        }

        var request = await repository.GetByIdAsync(requestId, ct);
        if (request is default(PersistedRequestEntity))
        {
            RestoreBody(http, flushResponse: true);
            return;
        }

        var statusCode = http.Response.StatusCode;
        var status = Classify(isStreaming, statusCode);
        var updated = request with
        {
            Status = status,
            ResponsePayload = responsePayload,
            ResponseStatusCode = statusCode,
            LeaseUntil = null,
            NextAttemptAt = status == RequestPersistenceStatus.RetryNeeded
                ? options.Value.ComputeBackoff(request.RetryCount, now)
                : null,
            UpdatedAt = now
        };

        // Completion marker must be durable before the client learns anything.
        await repository.UpdateAsync(updated, ct);

        logger.LogInformation(
            "[PersistRequest] {RequestId} finished: status={Status} http={StatusCode} retries={Retries} payloadCaptured={Captured}"
            + " — marker durable, flushing response to client.",
            requestId, status, statusCode, updated.RetryCount, !string.IsNullOrWhiteSpace(responsePayload));

        RestoreBody(http, flushResponse: true);

        await PersistRequestRetentionPolicy.ApplyAsync(repository, options.Value, updated, now, ct);
        logger.LogInformation(
            "[PersistRequest] {RequestId} retention: tombstoned={Tombstoned} (plainFirstAttempt={Plain}, tombstoneEnabled={Enabled}, keepRecovered={KeepRecovered}).",
            requestId,
            options.Value.TombstonePlainFinishes && updated.RetryCount == 0,
            updated.RetryCount == 0,
            options.Value.TombstonePlainFinishes,
            options.Value.KeepRecoveredForAudit);
    }

    private static RequestPersistenceStatus Classify(bool isStreaming, int statusCode)
    {
        if (isStreaming)
        {
            return RequestPersistenceStatus.Succeeded;
        }

        return statusCode switch
        {
            >= 200 and < 300 => RequestPersistenceStatus.Succeeded,
            >= 400 and < 500 => RequestPersistenceStatus.Failed,
            >= 500 or 429 => RequestPersistenceStatus.RetryNeeded,
            _ => RequestPersistenceStatus.Failed
        };
    }

    private static void RestoreBody(HttpContext http, bool flushResponse)
    {
        if (!http.Items.TryGetValue(ItemsKeyOriginalBody, out var originalObj) || originalObj is not Stream original)
        {
            return;
        }

        http.Response.Body = original;
        http.Items.Remove(ItemsKeyOriginalBody);

        if (flushResponse
            && http.Items.TryGetValue(ItemsKeyCaptureStream, out var captureObj)
            && captureObj is MemoryStream capture)
        {
            capture.Position = 0;
            capture.CopyTo(original, bufferSize: 64 * 1024);
            http.Items.Remove(ItemsKeyCaptureStream);
        }
    }
}
