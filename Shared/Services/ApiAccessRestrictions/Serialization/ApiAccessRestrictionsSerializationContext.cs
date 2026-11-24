using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.ApiAccessRestrictions;

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
[JsonSerializable(typeof(Core.Entities.ApiAccessRestrictions.ApiAccessRuleEntity))]
[JsonSerializable(typeof(List<Core.Entities.ApiAccessRestrictions.ApiAccessRuleEntity>))]
[JsonSerializable(typeof(Core.Entities.ApiAccessRestrictions.ApiAccessResponseTemplateEntity))]
[JsonSerializable(typeof(List<Core.Entities.ApiAccessRestrictions.ApiAccessResponseTemplateEntity>))]
[JsonSerializable(typeof(Core.Entities.ApiAccessRestrictions.IpRestrictionEntity))]
[JsonSerializable(typeof(Core.Entities.ApiAccessRestrictions.RegionRestrictionEntity))]
[JsonSerializable(typeof(Core.Entities.ApiAccessRestrictions.RestrictionLevel))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.CreateApiAccessRuleDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.UpdateApiAccessRuleDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.ApiAccessRuleDto))]
[JsonSerializable(typeof(List<PublicApi.BusinessObjects.Dto.ApiAccessRuleDto>))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.CreateApiAccessResponseTemplateDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.UpdateApiAccessResponseTemplateDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.ApiAccessResponseTemplateDto))]
[JsonSerializable(typeof(List<PublicApi.BusinessObjects.Dto.ApiAccessResponseTemplateDto>))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.CheckApiAccessRequestDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.CheckApiAccessResponseDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.ApiAccessRestrictionsSnapshotDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.ApiAccessRuleSnapshotDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.ApiAccessResponseTemplateSnapshotDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.IpRestrictionSnapshotDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.RegionRestrictionSnapshotDto))]
[JsonSerializable(typeof(AspireYarpGateway.Core.Contracts.CheckIpRequest))]
[JsonSerializable(typeof(AspireYarpGateway.Core.Contracts.CheckIpResponse))]
[JsonSerializable(typeof(AspireYarpGateway.Core.Contracts.ApiAccessRestrictionsChanged))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class ApiAccessRestrictionsSerializationContext : JsonSerializerContext;
