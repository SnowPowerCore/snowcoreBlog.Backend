using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.ApiAccessRestrictions;

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
[JsonSerializable(typeof(global::snowcoreBlog.Backend.ApiAccessRestrictions.Core.Entities.ApiAccessRestrictions.ApiAccessRuleEntity))]
[JsonSerializable(typeof(global::System.Collections.Generic.List<global::snowcoreBlog.Backend.ApiAccessRestrictions.Core.Entities.ApiAccessRestrictions.ApiAccessRuleEntity>))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.ApiAccessRestrictions.Core.Entities.ApiAccessRestrictions.ApiAccessResponseTemplateEntity))]
[JsonSerializable(typeof(global::System.Collections.Generic.List<global::snowcoreBlog.Backend.ApiAccessRestrictions.Core.Entities.ApiAccessRestrictions.ApiAccessResponseTemplateEntity>))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.ApiAccessRestrictions.Core.Entities.ApiAccessRestrictions.IpRestrictionEntity))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.ApiAccessRestrictions.Core.Entities.ApiAccessRestrictions.RegionRestrictionEntity))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.ApiAccessRestrictions.Core.Entities.ApiAccessRestrictions.RestrictionLevel))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.CreateApiAccessRuleDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.UpdateApiAccessRuleDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.ApiAccessRuleDto))]
[JsonSerializable(typeof(global::System.Collections.Generic.List<global::snowcoreBlog.PublicApi.BusinessObjects.Dto.ApiAccessRuleDto>))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.CreateApiAccessResponseTemplateDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.UpdateApiAccessResponseTemplateDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.ApiAccessResponseTemplateDto))]
[JsonSerializable(typeof(global::System.Collections.Generic.List<global::snowcoreBlog.PublicApi.BusinessObjects.Dto.ApiAccessResponseTemplateDto>))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.CheckApiAccessRequestDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.CheckApiAccessResponseDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.ApiAccessRestrictionsSnapshotDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.ApiAccessRuleSnapshotDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.ApiAccessResponseTemplateSnapshotDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.IpRestrictionSnapshotDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.RegionRestrictionSnapshotDto))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts.CheckIpRequest))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts.CheckIpResponse))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts.ApiAccessRestrictionsChanged))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class ApiAccessRestrictionsSerializationContext : JsonSerializerContext;
