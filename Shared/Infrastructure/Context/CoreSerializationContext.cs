using System.Text.Json;
using System.Text.Json.Serialization;
using MassTransit;
using MassTransit.Events;
using Scalar.AspNetCore;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.Infrastructure;

/// <summary>
/// Defines the serialization context.
/// </summary>
[JsonSerializable(typeof(ScalarOptions))]
[JsonSerializable(typeof(ApiResponse))]
[JsonSerializable(typeof(CreateAdmin))]
[JsonSerializable(typeof(CreateUser))]
[JsonSerializable(typeof(ValidateUserExists))]
[JsonSerializable(typeof(CreateReaderAccountDto))]
[JsonSerializable(typeof(ReaderAccountCreationResultDto))]
[JsonSerializable(typeof(Fault<UserCreationResult>))]
[JsonSerializable(typeof(FaultEvent<UserCreationResult>))]
[JsonSerializable(typeof(DataResult<UserCreationResult>))]
[JsonSerializable(typeof(Fault<ValidateUserExists>))]
[JsonSerializable(typeof(FaultEvent<ValidateUserExists>))]
[JsonSerializable(typeof(Fault<UserExistsValidationResult>))]
[JsonSerializable(typeof(FaultEvent<UserExistsValidationResult>))]
[JsonSerializable(typeof(DataResult<UserExistsValidationResult>))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class CoreSerializationContext : JsonSerializerContext { }