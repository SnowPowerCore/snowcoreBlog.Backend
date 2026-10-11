using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using FastEndpoints;

namespace snowcoreBlog.Backend.Infrastructure.StepProgress;

/// <summary>
/// A <see cref="StreamItem"/> that serializes its payload through a
/// source-generated <see cref="JsonTypeInfo{T}"/> instead of the global
/// reflection-based serializer options, keeping server-sent events
/// trimming/AOT-safe.
/// <para>
/// The payload is always written as a single compact line of JSON: the SSE
/// data field cannot span multiple lines, while the shared serialization
/// context is configured with <c>WriteIndented = true</c>.
/// </para>
/// </summary>
public sealed class JsonStreamItem<T>(string eventName, T data, JsonTypeInfo<T> typeInfo, string? id = null)
    : StreamItem(id, eventName, data!)
{
    /// <inheritdoc />
    public override string GetDataString(JsonSerializerOptions options)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer, new JsonWriterOptions { Indented = false });
        JsonSerializer.Serialize(writer, data, typeInfo);
        writer.Flush();

        var span = buffer.WrittenSpan;
        return Encoding.UTF8.GetString(span);
    }
}
