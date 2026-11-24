using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.UrlShortener;

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
[JsonSerializable(typeof(global::snowcoreBlog.Backend.UrlShortener.Core.Entities.UrlShortener.UrlMappingEntity))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.UrlShortener.Core.Entities.UrlShortener.ClickEventEntity))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.CreateShortUrlRequest))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.CreateShortUrlResult))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.ResolveShortUrlResultDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.ShortUrlStatsDto))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class UrlShortenerSerializationContext : JsonSerializerContext;
