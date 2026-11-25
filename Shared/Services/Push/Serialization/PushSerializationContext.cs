using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.Push;

/// <summary>
/// Service-owned serialization context. Registers every type this service serializes
/// (entities, contracts, DTOs, MassTransit envelopes). Shared runtime types live in
/// <see cref="Infrastructure.InfrastructureSerializationContext"/>,
/// which is chained in by <c>SetJsonSerializationContext</c>.
/// </summary>
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(List<byte>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(List<FluentValidation.Results.ValidationFailure>))]
[JsonSerializable(typeof(PublicApi.Utilities.Api.ApiResponse))]
[JsonSerializable(typeof(PublicApi.Utilities.Sse.StepProgressEvent))]
[JsonSerializable(typeof(PublicApi.Utilities.Sse.StepProgressFinalEvent))]
[JsonSerializable(typeof(Core.Contracts.GenericPushSent))]
[JsonSerializable(typeof(Core.Contracts.SendGenericPush))]
[JsonSerializable(typeof(MassTransit.Events.FaultEvent<Core.Contracts.GenericPushSent>))]
[JsonSerializable(typeof(MassTransit.Fault<Core.Contracts.GenericPushSent>))]
[JsonSerializable(typeof(PublicApi.Utilities.DataResult.DataResult<Core.Contracts.GenericPushSent>))]
[JsonSerializable(typeof(MassTransit.Events.FaultEvent<Core.Contracts.SendGenericPush>))]
[JsonSerializable(typeof(MassTransit.Fault<Core.Contracts.SendGenericPush>))]
[JsonSerializable(typeof(PublicApi.Utilities.DataResult.DataResult<Core.Contracts.SendGenericPush>))]
// Base action type only: it is the declared member type of SendGenericPush.Actions.
// Derived subclasses (broadcast/http/view) are Mapperly mapping inputs, never JSON
// payloads — registering them trips a STJ source-generator bug (virtual/override
// `Type` property → duplicate __value_Type local, CS0128).
[JsonSerializable(typeof(Core.Contracts.NotificationAction))]
[JsonSerializable(typeof(Core.Contracts.NotificationAttachment))]
[JsonSerializable(typeof(Core.Contracts.NotificationPriority))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class PushSerializationContext : JsonSerializerContext;
