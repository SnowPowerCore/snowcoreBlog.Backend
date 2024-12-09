using System.Text.Json;
using System.Text.Json.Serialization;
using Ixnas.AltchaNet;
using MassTransit;
using MassTransit.Events;
using Scalar.AspNetCore;
using snowcoreBlog.Backend.Core.Entities.Reader;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.Email.Core.Models.Email;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.Infrastructure;

/// <summary>
/// Defines the serialization context.
/// </summary>
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(ScalarOptions))]
[JsonSerializable(typeof(ApiResponse))]
[JsonSerializable(typeof(ApplicationAdminEntity))]
[JsonSerializable(typeof(ApplicationUserEntity))]
[JsonSerializable(typeof(ApplicationTempUserEntity))]
[JsonSerializable(typeof(ReaderEntity))]
[JsonSerializable(typeof(CreateAdmin))]
[JsonSerializable(typeof(CreateUser))]
[JsonSerializable(typeof(CreateTempUser))]
[JsonSerializable(typeof(ValidateUserExists))]
[JsonSerializable(typeof(ValidateTempUserExists))]
[JsonSerializable(typeof(ValidateUserNickNameTaken))]
[JsonSerializable(typeof(ReaderAccountTempUserCreated))]
[JsonSerializable(typeof(RequestCreateReaderAccountDto))]
[JsonSerializable(typeof(RequestReaderAccountCreationResultDto))]
[JsonSerializable(typeof(CheckNickNameNotTakenDto))]
[JsonSerializable(typeof(NickNameNotTakenCheckResultDto))]
[JsonSerializable(typeof(Fault<UserCreationResult>))]
[JsonSerializable(typeof(Fault<TempUserCreationResult>))]
[JsonSerializable(typeof(Fault<UserNickNameTakenValidationResult>))]
[JsonSerializable(typeof(Fault<EmailDomainChecked>))]
[JsonSerializable(typeof(Fault<ReaderAccountTempUserCreated>))]
[JsonSerializable(typeof(FaultEvent<UserCreationResult>))]
[JsonSerializable(typeof(FaultEvent<TempUserCreationResult>))]
[JsonSerializable(typeof(FaultEvent<UserNickNameTakenValidationResult>))]
[JsonSerializable(typeof(FaultEvent<EmailDomainChecked>))]
[JsonSerializable(typeof(FaultEvent<ReaderAccountTempUserCreated>))]
[JsonSerializable(typeof(DataResult<UserCreationResult>))]
[JsonSerializable(typeof(DataResult<TempUserCreationResult>))]
[JsonSerializable(typeof(DataResult<UserNickNameTakenValidationResult>))]
[JsonSerializable(typeof(DataResult<EmailDomainChecked>))]
[JsonSerializable(typeof(Fault<ValidateUserExists>))]
[JsonSerializable(typeof(Fault<ValidateTempUserExists>))]
[JsonSerializable(typeof(FaultEvent<ValidateUserExists>))]
[JsonSerializable(typeof(FaultEvent<ValidateTempUserExists>))]
[JsonSerializable(typeof(Fault<UserExistsValidationResult>))]
[JsonSerializable(typeof(Fault<TempUserExistsValidationResult>))]
[JsonSerializable(typeof(FaultEvent<UserExistsValidationResult>))]
[JsonSerializable(typeof(FaultEvent<TempUserExistsValidationResult>))]
[JsonSerializable(typeof(DataResult<UserExistsValidationResult>))]
[JsonSerializable(typeof(DataResult<TempUserExistsValidationResult>))]
[JsonSerializable(typeof(AltchaChallenge))]
[JsonSerializable(typeof(CheckEmailDomain))]
[JsonSerializable(typeof(SendGenericEmail))]
[JsonSerializable(typeof(ActivateCreatedTempUserData))]
[JsonSerializable(typeof(SendTemplatedEmail<ActivateCreatedTempUserData>))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class CoreSerializationContext : JsonSerializerContext { }