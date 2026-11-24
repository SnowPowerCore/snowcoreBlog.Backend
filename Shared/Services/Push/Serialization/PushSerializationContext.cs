using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.Push;

/// <summary>
/// Service-owned serialization context. Registers every type this service serializes
/// (entities, contracts, DTOs, MassTransit envelopes). Shared runtime types live in
/// <see cref="snowcoreBlog.Backend.Infrastructure.InfrastructureSerializationContext"/>,
/// which is chained in by <c>SetJsonSerializationContext</c>.
/// </summary>
[JsonSerializable(typeof(global::System.Collections.Generic.List<string>))]
[JsonSerializable(typeof(global::System.Collections.Generic.List<byte>))]
[JsonSerializable(typeof(global::System.Collections.Generic.Dictionary<string, string>))]
[JsonSerializable(typeof(global::System.Text.Json.JsonElement))]
[JsonSerializable(typeof(global::System.Collections.Generic.List<global::FluentValidation.Results.ValidationFailure>))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.Utilities.Api.ApiResponse))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.Utilities.Sse.StepProgressEvent))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.Utilities.Sse.StepProgressFinalEvent))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.Push.Core.Contracts.GenericPushSent))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.Push.Core.Contracts.SendGenericPush))]
[JsonSerializable(typeof(global::MassTransit.Events.FaultEvent<global::snowcoreBlog.Backend.Push.Core.Contracts.GenericPushSent>))]
[JsonSerializable(typeof(global::MassTransit.Fault<global::snowcoreBlog.Backend.Push.Core.Contracts.GenericPushSent>))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.Utilities.DataResult.DataResult<global::snowcoreBlog.Backend.Push.Core.Contracts.GenericPushSent>))]
[JsonSerializable(typeof(global::MassTransit.Events.FaultEvent<global::snowcoreBlog.Backend.Push.Core.Contracts.SendGenericPush>))]
[JsonSerializable(typeof(global::MassTransit.Fault<global::snowcoreBlog.Backend.Push.Core.Contracts.SendGenericPush>))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.Utilities.DataResult.DataResult<global::snowcoreBlog.Backend.Push.Core.Contracts.SendGenericPush>))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class PushSerializationContext : JsonSerializerContext;
