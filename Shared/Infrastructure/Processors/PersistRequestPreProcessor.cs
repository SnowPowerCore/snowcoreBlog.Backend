using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using snowcoreBlog.Backend.Shared.Core.Entities.RequestPersistence;
using snowcoreBlog.Backend.Shared.Core.Interfaces.Repositories;
using snowcoreBlog.Backend.Infrastructure.RequestPersistence;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.Infrastructure.Processors;

/// <summary>
/// Durable claim + duplicate short-circuit for persist-tagged endpoints. The incoming request is
/// serialized and durably committed (independent session) before any business logic runs; if the
/// commit fails the request is refused (503) so unpersisted requests never execute.
/// <para>
/// Existing claims: payload-hash mismatch в†’ conflict-informative; Succeeded/Failed в†’ completed-
/// informative (stored business response is never returned); fresh Processing lease в†’ in-progress-
/// informative; stale/takeover-able в†’ atomic takeover + resume scope for the checkpoint advices.
/// </para>
/// <para>
/// The response body of non-streaming endpoints is captured (stream swap) so the post-processor
/// can persist the response only after the completion marker is durable.
/// </para>
/// </summary>
/// <remarks>
/// FastEndpoints constructs global processor instances as singletons from the ROOT provider вЂ”
/// scoped services must be resolved per request via <see cref="IServiceScopeFactory"/>.
/// </remarks>
public class PersistRequestPreProcessor(
    IServiceScopeFactory scopeFactory,
    IOptions<PersistRequestOptions> options,
    ILogger<PersistRequestPreProcessor> logger) : IGlobalPreProcessor
{
    private const string ItemsKeyRequestId = "__PersistRequestId";
    private const string ItemsKeyIsStreaming = "__PersistIsStreaming";
    private const string ItemsKeyCaptureStream = "__PersistCaptureStream";
    private const string ItemsKeyOriginalBody = "__PersistOriginalBody";

    private static readonly JsonSerializerOptions SnapshotOptions = new(JsonSerializerDefaults.Web)
    {
        TypeInfoResolver = CoreSerializationContext.Default
    };

    private static readonly string[] AuthRelevantHeaders = ["Cookie", "Authorization"];

    public async Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
    {
        var http = context.HttpContext;

        if (!http.Request.Headers.TryGetValue(HeaderKeyConstants.IdempotencyKey, out var keyHeader)
            || string.IsNullOrWhiteSpace(keyHeader))
        {
            context.ValidationFailures.Add(new("IdempotencyKey", $"The [{HeaderKeyConstants.IdempotencyKey}] header has to be set!"));
            await http.Response.SendErrorsAsync(context.ValidationFailures, cancellation: ct);
            return;
        }

        await using var scope = scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRequestPersistenceRepository>();

        var idempotencyKey = keyHeader.ToString();
        var isStreaming = ProducesEventStream(http.GetEndpoint());
        var now = DateTimeOffset.UtcNow;

        http.Request.EnableBuffering();
        var requestBody = await ReadRequestBodyAsync(http.Request, ct);
        var requestHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(requestBody)));

        var claim = new PersistedRequestEntity
        {
            Id = Guid.CreateVersion7(),
            RequestId = Guid.Empty,
            IdempotencyKey = idempotencyKey,
            RequestHash = requestHash,
            Status = RequestPersistenceStatus.Persisted,
            Method = http.Request.Method,
            Path = http.Request.Path.Value ?? string.Empty,
            QueryString = http.Request.QueryString.Value ?? string.Empty,
            ContentType = http.Request.ContentType ?? string.Empty,
            HeadersJson = JsonSerializer.Serialize(CaptureAuthHeaders(http.Request), SnapshotOptions),
            IsStreamingResponse = isStreaming,
            RequestPayload = requestBody,
            CreatedAt = now,
            UpdatedAt = now
        };
        claim = claim with { RequestId = claim.Id };

        // Check-first: known duplicates short-circuit WITHOUT touching the insert path (Marten
        // logs command exceptions as `fail:` even when handled вЂ” avoid the noise entirely).
        var existing = await repository.GetByIdempotencyKeyAsync(idempotencyKey, ct);
        if (existing is default(PersistedRequestEntity))
        {
            try
            {
                await repository.CreateAsync(claim, ct);
            }
            catch (Exception ex) when (IsUniqueViolation(ex))
            {
                // Concurrent first-arrival race вЂ” converge on the winner's row.
                existing = await repository.GetByIdempotencyKeyAsync(idempotencyKey, ct);
                if (existing is default(PersistedRequestEntity))
                {
                    throw;
                }
            }
        }

        if (existing is not default(PersistedRequestEntity))
        {
            logger.LogInformation(
                "[PersistRequest] Duplicate key {Key} вЂ” existing row {RequestId} status={Status} hashMatch={HashMatch}",
                idempotencyKey, existing.Id, existing.Status,
                string.Equals(existing.RequestHash, requestHash, StringComparison.Ordinal));
            await HandleExistingAsync(repository, http, existing, requestHash, isStreaming, now, ct);
            return;
        }

        logger.LogInformation(
            "[PersistRequest] Claim persisted durably: {RequestId} key={Key} streaming={Streaming} вЂ” entering execution",
            claim.Id, idempotencyKey, isStreaming);

        // Fresh claim вЂ” transition into the Processing lease. The claim commit above is already
        // durable; losing this transition sends the request to the recovery worker (T1 window).
        if (!await repository.TryTakeOverAsync(claim.Id, claim.RetryCount, now, options.Value.LeaseTtl, ct))
        {
            await SendIdempotencyStatusAsync(
                http, IdempotencyRequestState.InProgress, claim.Id, claim.CreatedAt,
                "The request is already being processed.", StatusCodes.Status202Accepted, ct);
            return;
        }

        http.Items[ItemsKeyRequestId] = claim.Id;
        AttachCaptureStream(http, isStreaming);
    }

    private async Task HandleExistingAsync(
        IRequestPersistenceRepository repository,
        HttpContext http,
        PersistedRequestEntity existing,
        string requestHash,
        bool isStreaming,
        DateTimeOffset now,
        CancellationToken ct)
    {
        var opts = options.Value;

        if (!string.Equals(existing.RequestHash, requestHash, StringComparison.Ordinal))
        {
            await SendIdempotencyStatusAsync(
                http, IdempotencyRequestState.Conflict, existing.Id, existing.CreatedAt,
                "Idempotency key was already used with a different request payload.",
                StatusCodes.Status202Accepted, ct);
            return;
        }

        switch (existing.Status)
        {
            case RequestPersistenceStatus.Succeeded:
                await SendIdempotencyStatusAsync(
                    http, IdempotencyRequestState.Completed, existing.Id, existing.CreatedAt,
                    "The request has already been completed and will not be executed again.",
                    StatusCodes.Status200OK, ct);
                return;

            case RequestPersistenceStatus.Failed:
                await SendIdempotencyStatusAsync(
                    http, IdempotencyRequestState.Completed, existing.Id, existing.CreatedAt,
                    "The request failed definitively on an earlier attempt; manual handling is required.",
                    StatusCodes.Status200OK, ct);
                return;

            case RequestPersistenceStatus.Processing when existing.LeaseUntil > now:
                await SendIdempotencyStatusAsync(
                    http, IdempotencyRequestState.InProgress, existing.Id, existing.CreatedAt,
                    "The request is currently being processed.", StatusCodes.Status202Accepted, ct);
                return;

            case RequestPersistenceStatus.RetryNeeded when existing.NextAttemptAt is { } dueAt && dueAt > now:
                await SendIdempotencyStatusAsync(
                    http, IdempotencyRequestState.InProgress, existing.Id, existing.CreatedAt,
                    "The request is scheduled for automatic retry.", StatusCodes.Status202Accepted, ct);
                return;

            default:
                break; // Persisted / Received / stale Processing / due RetryNeeded в†’ takeover
        }

        // Eligibility was already checked above (stale Processing lease / Persisted / due
        // RetryNeeded); the takeover re-derives expiry comparison atomically.
        if (!await repository.TryTakeOverAsync(existing.Id, existing.RetryCount, now, opts.LeaseTtl, ct))
        {
            logger.LogInformation("[PersistRequest] Takeover of {RequestId} lost вЂ” another execution holds the lease.", existing.Id);
            await SendIdempotencyStatusAsync(
                http, IdempotencyRequestState.InProgress, existing.Id, existing.CreatedAt,
                "The request is currently being processed.", StatusCodes.Status202Accepted, ct);
            return;
        }

        // Won the takeover вЂ” resume from the highest contiguous checkpoint (if any).
        var checkpoints = await repository.GetCheckpointsAsync(existing.Id, ct);
        var contiguous = TakeContiguous(checkpoints);
        var lastCheckpoint = checkpoints.Count > 0 ? checkpoints[^1] : null;
        var last = contiguous.Count > 0 ? contiguous[^1] : null;

        logger.LogInformation(
            "[PersistRequest] Takeover of {RequestId} won вЂ” {Checkpoints} checkpoint(s), resuming from step index {ResumeIndex} ({ResumeStep}).",
            existing.Id, contiguous.Count, last?.StepIndex ?? -1, last?.StepTypeName ?? "<from scratch>");

        RequestResumeScope.Current = new RequestResumeScope
        {
            PersistedRequestId = existing.Id,
            PipelineKey = lastCheckpoint?.PipelineKey ?? "*",
            RestoreState = last is default(ResumeStepState)
                ? default
                : new ResumeStepState(last.StepIndex, last.StepTypeName, last.SnapshotType, last.SnapshotJson),
            CompletedStepTypes = contiguous.Select(static c => c.StepTypeName).ToHashSet(StringComparer.Ordinal)
        };

        http.Items[ItemsKeyRequestId] = existing.Id;
        AttachCaptureStream(http, isStreaming);
    }

    private static void AttachCaptureStream(HttpContext http, bool isStreaming)
    {
        http.Items[ItemsKeyIsStreaming] = isStreaming;
        if (isStreaming)
        {
            return;
        }

        var original = http.Response.Body;
        var capture = new MemoryStream();
        http.Response.Body = capture;
        http.Items[ItemsKeyCaptureStream] = capture;
        http.Items[ItemsKeyOriginalBody] = original;
    }

    private static List<ResumeStepState> TakeContiguous(IReadOnlyList<RequestCheckpointEntity> checkpoints)
    {
        var contiguous = new List<ResumeStepState>();
        var expected = 0;
        foreach (var checkpoint in checkpoints)
        {
            if (checkpoint.StepIndex != expected)
            {
                break;
            }

            contiguous.Add(new ResumeStepState(checkpoint.StepIndex, checkpoint.StepTypeName, checkpoint.SnapshotType, checkpoint.SnapshotJson));
            expected++;
        }

        return contiguous;
    }

    private static async Task SendIdempotencyStatusAsync(
        HttpContext http,
        IdempotencyRequestState state,
        Guid persistedRequestId,
        DateTimeOffset originalCreatedAt,
        string message,
        int statusCode,
        CancellationToken ct)
    {
        http.Response.StatusCode = statusCode;
        await http.Response.WriteAsJsonAsync(
            new IdempotencyRequestStatusDto(state, persistedRequestId, originalCreatedAt, message),
            SnapshotOptions,
            cancellationToken: ct);
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request, CancellationToken ct)
    {
        try
        {
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var body = await reader.ReadToEndAsync(ct);
            request.Body.Position = 0;
            return body;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static Dictionary<string, string> CaptureAuthHeaders(HttpRequest request)
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in AuthRelevantHeaders)
        {
            if (request.Headers.TryGetValue(name, out var values))
            {
                headers[name] = values.ToString();
            }
        }

        return headers;
    }

    private static bool ProducesEventStream(Endpoint? endpoint)
    {
        if (endpoint is default(Endpoint))
        {
            return false;
        }

        foreach (var metadata in endpoint.Metadata)
        {
            if (metadata is ProducesResponseTypeMetadata produces
                && produces.ContentTypes.Contains("text/event-stream"))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsUniqueViolation(Exception ex)
    {
        for (var current = (Exception?)ex; current is not default(Exception); current = current.InnerException)
        {
            if (current is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                return true;
            }

            // Marten wraps Npgsql failures in MartenCommandException вЂ” keep a message fallback
            // in case the inner-chain shape changes across versions.
            if (current.Message.Contains("23505", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}
