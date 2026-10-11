using System.Runtime.CompilerServices;
using System.Net;
using System.Threading.Channels;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Utilities.Sse;

namespace snowcoreBlog.Backend.Infrastructure.StepProgress;

/// <summary>
/// Generic server-sent events support for endpoints that execute a long-running
/// process (typically a stepified one) and want to report the progress of its
/// meaningful steps to the client in real time.
/// <para>
/// The abstraction is domain-agnostic: the endpoint supplies the process itself —
/// any delegate that reports step codes through the attached
/// <see cref="StepProgressReporter"/> and produces the regular <see cref="ApiResponse"/>
/// envelope plus the status code the process would have produced with a plain JSON
/// response. The steps push plain string codes; this extension streams them as
/// <see cref="StepProgressEvent"/> frames and always terminates the stream with a
/// single <see cref="StepProgressFinalEvent"/> frame carrying the response envelope.
/// </para>
/// <para>
/// Flushing policy: no intermediate flushes are issued by this extension. The
/// underlying FastEndpoints event stream writer flushes the buffered events and
/// completes the connection once the process (and therefore the stream) is over.
/// </para>
/// </summary>
public static class StepProgressSseExtensions
{
    /// <summary>
    /// Executes the process while streaming its step progress to the client.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> of the endpoint.</param>
    /// <param name="executeProcess">
    /// The process to execute. It receives the freshly created <see cref="StepProgressReporter"/>
    /// (to be attached to the process context by the endpoint) and must return the regular
    /// API response envelope of the process together with its status code.
    /// </param>
    /// <param name="cancellationToken">The request abort token of the endpoint.</param>
    public static async Task SendStepProgressStreamAsync(
        this HttpContext httpContext,
        Func<StepProgressReporter, CancellationToken, Task<(ApiResponse? Response, int StatusCode)>> executeProcess,
        CancellationToken cancellationToken = default)
    {
        var reporter = new StepProgressReporter();

        var processTask = ExecuteProcessAndProduceFinalFrameAsync(
            executeProcess, reporter, cancellationToken);

        await httpContext.Response.SendEventStreamAsync(
            StreamProgressAsync(reporter.Reader, processTask, cancellationToken),
            cancellationToken);

        // Observe the process outcome so no exception is left unobserved.
        await processTask;
    }

    private static async Task<StreamItem?> ExecuteProcessAndProduceFinalFrameAsync(
        Func<StepProgressReporter, CancellationToken, Task<(ApiResponse? Response, int StatusCode)>> executeProcess,
        StepProgressReporter reporter,
        CancellationToken cancellationToken)
    {
        TaskCompletionSource<StreamItem?> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        _ = RunProcessAsync();
        return await completion.Task;

        async Task RunProcessAsync()
        {
            StreamItem? finalFrame;
            try
            {
                var (response, statusCode) = await executeProcess(reporter, cancellationToken);
                finalFrame = CreateFinalFrame(response, statusCode);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // The client is gone; end the stream without a final frame.
                finalFrame = null;
            }
            catch (Exception exception)
            {
                finalFrame = CreateFinalFrame(
                    new ApiResponse(default, 0, -1, [exception.Message]),
                    (int)HttpStatusCode.InternalServerError);
            }
            finally
            {
                // Completing the writer lets draining readers finish after they
                // have consumed every reported step code.
                reporter.Complete();
            }

            completion.TrySetResult(finalFrame);
        }
    }

    private static StreamItem CreateFinalFrame(ApiResponse? response, int statusCode) =>
        new JsonStreamItem<StepProgressFinalEvent>(
            StepProgressEventConstants.FinalResultEvent,
            new StepProgressFinalEvent(statusCode, response),
            CoreSerializationContext.Default.StepProgressFinalEvent);

    private static async IAsyncEnumerable<StreamItem> StreamProgressAsync(
        ChannelReader<string> stepCodes,
        Task<StreamItem?> finalFrameTask,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var stepCode in stepCodes.ReadAllAsync(cancellationToken))
        {
            yield return new JsonStreamItem<StepProgressEvent>(
                StepProgressEventConstants.StepProgressEvent,
                new StepProgressEvent(stepCode),
                CoreSerializationContext.Default.StepProgressEvent);
        }

        var finalFrame = await finalFrameTask;
        if (finalFrame is not null)
        {
            yield return finalFrame;
        }
    }
}
