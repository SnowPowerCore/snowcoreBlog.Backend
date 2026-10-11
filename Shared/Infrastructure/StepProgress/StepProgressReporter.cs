using System.Threading.Channels;

namespace snowcoreBlog.Backend.Infrastructure.StepProgress;

/// <summary>
/// A flow-agnostic, channel-backed broadcaster of step progress codes.
/// A stepified process (or any other long-running operation) writes stable,
/// machine-readable step codes into it, while the endpoint concurrently reads
/// them and pushes them to the client as server-sent events.
/// <para>
/// The reporter is not bound to any domain: it transports plain strings, so any
/// flow or endpoint can adopt it without touching this infrastructure.
/// </para>
/// </summary>
public sealed class StepProgressReporter
{
    private readonly Channel<string> _channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = false,
        AllowSynchronousContinuations = false
    });

    /// <summary>
    /// Gets the reader side of the underlying channel for stream consumers.
    /// </summary>
    public ChannelReader<string> Reader => _channel.Reader;

    /// <summary>
    /// Reports that the process entered the step identified by <paramref name="stepCode"/>.
    /// Best-effort: reporting never throws and is ignored once the reporter is completed.
    /// </summary>
    /// <param name="stepCode">A stable, machine-readable step code without personal data.</param>
    public void Report(string stepCode) => _channel.Writer.TryWrite(stepCode);

    /// <summary>
    /// Marks the stream as complete. Draining readers will still receive all
    /// previously reported codes before the enumeration finishes.
    /// </summary>
    public void Complete() => _channel.Writer.TryComplete();
}
