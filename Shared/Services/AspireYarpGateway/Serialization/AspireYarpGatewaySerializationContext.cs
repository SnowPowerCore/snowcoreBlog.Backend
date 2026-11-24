using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.AspireYarpGateway;

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
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.ApiAccessRestrictionsSnapshotDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.ApiAccessRuleSnapshotDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.ApiAccessResponseTemplateSnapshotDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.IpRestrictionSnapshotDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.RegionRestrictionSnapshotDto))]
[JsonSerializable(typeof(Core.Contracts.CheckIpRequest))]
[JsonSerializable(typeof(Core.Contracts.CheckIpResponse))]
[JsonSerializable(typeof(Core.Contracts.ApiAccessRestrictionsChanged))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class AspireYarpGatewaySerializationContext : JsonSerializerContext;
