using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.Documents;

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
[JsonSerializable(typeof(Core.Entities.Documents.DocumentEntity))]
[JsonSerializable(typeof(Core.Entities.Documents.DocumentStatus))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.DocumentDto))]
[JsonSerializable(typeof(List<PublicApi.BusinessObjects.Dto.DocumentDto>))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.DocumentAdminDto))]
[JsonSerializable(typeof(List<PublicApi.BusinessObjects.Dto.DocumentAdminDto>))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.DocumentStatusDto))]
[JsonSerializable(typeof(Core.Contracts.DocumentUploaded))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class DocumentsSerializationContext : JsonSerializerContext;
