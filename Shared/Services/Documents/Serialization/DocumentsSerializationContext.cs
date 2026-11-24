using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.Documents;

/// <summary>
/// Service-owned serialization context. Registers every type this service serializes
/// (entities, contracts, DTOs, MassTransit envelopes). Shared runtime types live in
/// <see cref="Backend.Infrastructure.InfrastructureSerializationContext"/>,
/// which is chained in by <c>SetJsonSerializationContext</c>.
/// </summary>
[JsonSerializable(typeof(global::System.Collections.Generic.List<string>))]
[JsonSerializable(typeof(global::System.Collections.Generic.List<byte>))]
[JsonSerializable(typeof(global::System.Collections.Generic.Dictionary<string, string>))]
[JsonSerializable(typeof(global::System.Text.Json.JsonElement))]
[JsonSerializable(typeof(global::System.Collections.Generic.List<global::FluentValidation.Results.ValidationFailure>))]
[JsonSerializable(typeof(snowcoreBlog.PublicApi.Utilities.Api.ApiResponse))]
[JsonSerializable(typeof(snowcoreBlog.PublicApi.Utilities.Sse.StepProgressEvent))]
[JsonSerializable(typeof(snowcoreBlog.PublicApi.Utilities.Sse.StepProgressFinalEvent))]
[JsonSerializable(typeof(Documents.Core.Entities.Documents.DocumentEntity))]
[JsonSerializable(typeof(Documents.Core.Entities.Documents.DocumentStatus))]
[JsonSerializable(typeof(snowcoreBlog.PublicApi.BusinessObjects.Dto.DocumentDto))]
[JsonSerializable(typeof(global::System.Collections.Generic.List<snowcoreBlog.PublicApi.BusinessObjects.Dto.DocumentDto>))]
[JsonSerializable(typeof(snowcoreBlog.PublicApi.BusinessObjects.Dto.DocumentAdminDto))]
[JsonSerializable(typeof(global::System.Collections.Generic.List<snowcoreBlog.PublicApi.BusinessObjects.Dto.DocumentAdminDto>))]
[JsonSerializable(typeof(snowcoreBlog.PublicApi.BusinessObjects.Dto.DocumentStatusDto))]
[JsonSerializable(typeof(Documents.Core.Contracts.DocumentUploaded))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class DocumentsSerializationContext : JsonSerializerContext;
