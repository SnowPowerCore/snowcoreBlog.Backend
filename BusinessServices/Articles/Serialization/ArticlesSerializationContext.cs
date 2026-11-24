using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.Articles;

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
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.ArticleDto))]
[JsonSerializable(typeof(List<PublicApi.BusinessObjects.Dto.ArticleDto>))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.ArticleAdminDto))]
[JsonSerializable(typeof(List<PublicApi.BusinessObjects.Dto.ArticleAdminDto>))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.ArticleAdminDetailDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.SetArticlePublicationDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.CreateArticleDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.CreateArticleResultDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.AntiforgeryResultDto))]
[JsonSerializable(typeof(Infrastructure.Entities.AltchaStoredChallengeEntity))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class ArticlesSerializationContext : JsonSerializerContext;
