using System.Text.Json.Serialization;
using snowcoreBlog.Backend.ReadersManagement.Context;

namespace snowcoreBlog.Backend.ReadersManagement;

/// <summary>
/// Source-generated serialization metadata for durable step checkpoints. Every snapshot type used
/// by a checkpointed pipeline in this service must be registered here (property types are resolved
/// transitively) — reflection-free, AOT-safe.
/// </summary>
[JsonSerializable(typeof(ConfirmCreateReaderAccountCheckpoint))]
internal sealed partial class ReadersCheckpointJsonContext : JsonSerializerContext;
