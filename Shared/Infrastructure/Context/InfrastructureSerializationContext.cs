using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation.Results;
using Ixnas.AltchaNet;
using snowcoreBlog.Backend.Infrastructure.Entities;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Utilities.Sse;

namespace snowcoreBlog.Backend.Infrastructure;

/// <summary>
/// Shared (cross-service) serialization context of the Infrastructure package. Registers only the
/// runtime types the shared infrastructure itself serializes (step progress, cookies/refresh-token
/// records, Altcha, idempotency, validation envelopes). Every service owns a
/// <c>&lt;Service&gt;SerializationContext</c> for its domain types and chains both together via
/// <c>SetJsonSerializationContext</c>.
/// </summary>
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(List<byte>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(ReaderRefreshTokenRecord))]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(List<ValidationFailure>))]
[JsonSerializable(typeof(ApiResponse))]
[JsonSerializable(typeof(StepProgressEvent))]
[JsonSerializable(typeof(StepProgressFinalEvent))]
[JsonSerializable(typeof(IdempotencyRequestStatusDto))]
[JsonSerializable(typeof(IdempotencyRequestState))]
[JsonSerializable(typeof(AltchaChallenge))]
[JsonSerializable(typeof(AltchaResponse))]
[JsonSerializable(typeof(AltchaStoredChallengeEntity))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class InfrastructureSerializationContext : JsonSerializerContext;
