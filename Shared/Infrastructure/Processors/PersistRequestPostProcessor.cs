using System.Text;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using snowcoreBlog.Backend.Core.Entities.RequestPersistence;
using snowcoreBlog.Backend.Core.Interfaces.Repositories;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.Infrastructure.Processors;

public class PersistRequestPostProcessor(IRequestPersistenceRepository repository) : IGlobalPostProcessor
{
    private const string ItemsKey_OriginalResponseBody = "__Persist_OriginalResponseBody";
    private const string ItemsKey_ResponseCaptureStream = "__Persist_ResponseCaptureStream";
    private const string ItemsKey_PersistedRequestId = "__PersistedRequestId";

    public async Task PostProcessAsync(IPostProcessorContext context, CancellationToken ct)
    {
        var http = context.HttpContext;

        if (!http.Request.Headers.TryGetValue(HeaderKeyConstants.PersistHeader, out var persistHeader) || string.IsNullOrWhiteSpace(persistHeader))
            return;

        // Ensure endpoint is tagged with Persist (best-effort via metadata inspection)
        var endpoint = http.GetEndpoint();
        var hasPersistTag = false;
        if (endpoint is not default(Endpoint))
        {
            foreach (var md in endpoint.Metadata)
            {
                if (md is string s && string.Equals(s, ApiTagConstants.Persist, StringComparison.OrdinalIgnoreCase))
                {
                    hasPersistTag = true;
                    break;
                }

                if (md is IEnumerable<string> seq && seq.Contains(ApiTagConstants.Persist))
                {
                    hasPersistTag = true;
                    break;
                }

                if (md?.ToString()?.Contains(ApiTagConstants.Persist, StringComparison.OrdinalIgnoreCase) == true)
                {
                    hasPersistTag = true;
                    break;
                }
            }
        }

        if (!hasPersistTag)
        {
            // Not an endpoint we should persist for; still restore response body if we captured it
            await RestoreResponseBodyAsync(http, ct);
            return;
        }

        if (!http.Items.TryGetValue(ItemsKey_PersistedRequestId, out var idObj) || idObj is not Guid persistedId)
        {
            await RestoreResponseBodyAsync(http, ct);
            return;
        }

        string? responsePayload = null;
        if (http.Items.TryGetValue(ItemsKey_ResponseCaptureStream, out var captureObj) && captureObj is MemoryStream capture)
        {
            try
            {
                capture.Position = 0;
                using var sr = new StreamReader(capture, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
                responsePayload = await sr.ReadToEndAsync(ct);
                capture.Position = 0;

                // Copy captured response back to original response stream
                if (http.Items.TryGetValue(ItemsKey_OriginalResponseBody, out var origObj) && origObj is Stream orig)
                {
                    await capture.CopyToAsync(orig, ct);
                    http.Response.Body = orig;
                }
            }
            catch
            {
                // ignore capture errors
            }
        }

        // Determine status mapping
        var statusCode = http.Response.StatusCode;
        RequestPersistenceStatus newStatus;

        if (statusCode >= 200 && statusCode < 300)
            newStatus = RequestPersistenceStatus.Succeeded;
        else if (statusCode == 202)
            newStatus = RequestPersistenceStatus.Processing;
        else if (statusCode == 429 || (statusCode >= 500 && statusCode <= 599))
            newStatus = RequestPersistenceStatus.RetryNeeded;
        else
            newStatus = RequestPersistenceStatus.Failed;

        if (newStatus is RequestPersistenceStatus.Succeeded)
        {
            // On success, delete the persisted request record to free storage and indicate completion.
            await repository.DeleteAsync(persistedId, ct);
        }
        else
        {
            await repository.UpdateStatusAsync(persistedId, newStatus, responsePayload, ct);
        }
    }

    private static async Task RestoreResponseBodyAsync(HttpContext http, CancellationToken ct)
    {
        if (http.Items.TryGetValue(ItemsKey_ResponseCaptureStream, out var captureObj) && captureObj is MemoryStream capture)
        {
            if (http.Items.TryGetValue(ItemsKey_OriginalResponseBody, out var origObj) && origObj is Stream orig)
            {
                try
                {
                    capture.Position = 0;
                    await capture.CopyToAsync(orig, ct);
                    http.Response.Body = orig;
                }
                catch
                {
                    // ignore
                }
            }
        }
    }
}