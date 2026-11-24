using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.Email;

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
[JsonSerializable(typeof(Core.Contracts.CheckEmailDomain))]
[JsonSerializable(typeof(Core.Contracts.EmailDomainChecked))]
[JsonSerializable(typeof(Core.Contracts.SendGenericEmail))]
[JsonSerializable(typeof(Core.Contracts.GenericEmailSent))]
[JsonSerializable(typeof(Core.Contracts.SendTemplatedEmail))]
[JsonSerializable(typeof(Core.Contracts.TemplatedEmailSent))]
[JsonSerializable(typeof(MassTransit.Events.FaultEvent<Core.Contracts.EmailDomainChecked>))]
[JsonSerializable(typeof(MassTransit.Fault<Core.Contracts.EmailDomainChecked>))]
[JsonSerializable(typeof(PublicApi.Utilities.DataResult.DataResult<Core.Contracts.EmailDomainChecked>))]
[JsonSerializable(typeof(MassTransit.Events.FaultEvent<Core.Contracts.SendGenericEmail>))]
[JsonSerializable(typeof(MassTransit.Fault<Core.Contracts.SendGenericEmail>))]
[JsonSerializable(typeof(PublicApi.Utilities.DataResult.DataResult<Core.Contracts.SendGenericEmail>))]
[JsonSerializable(typeof(MassTransit.Events.FaultEvent<Core.Contracts.GenericEmailSent>))]
[JsonSerializable(typeof(MassTransit.Fault<Core.Contracts.GenericEmailSent>))]
[JsonSerializable(typeof(PublicApi.Utilities.DataResult.DataResult<Core.Contracts.GenericEmailSent>))]
[JsonSerializable(typeof(MassTransit.Events.FaultEvent<Core.Contracts.SendTemplatedEmail>))]
[JsonSerializable(typeof(MassTransit.Fault<Core.Contracts.SendTemplatedEmail>))]
[JsonSerializable(typeof(PublicApi.Utilities.DataResult.DataResult<Core.Contracts.SendTemplatedEmail>))]
[JsonSerializable(typeof(MassTransit.Events.FaultEvent<Core.Contracts.TemplatedEmailSent>))]
[JsonSerializable(typeof(MassTransit.Fault<Core.Contracts.TemplatedEmailSent>))]
[JsonSerializable(typeof(PublicApi.Utilities.DataResult.DataResult<Core.Contracts.TemplatedEmailSent>))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class EmailSerializationContext : JsonSerializerContext;
