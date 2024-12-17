using System.Text.Json;
using System.Text.Json.Serialization;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Ixnas.AltchaNet;
using MassTransit;
using MassTransit.Events;
using Microsoft.AspNetCore.Antiforgery;
using Scalar.AspNetCore;
using snowcoreBlog.Backend.Core.Contracts;
using snowcoreBlog.Backend.Core.Entities.Reader;
using snowcoreBlog.Backend.Email.Core.Contracts;
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
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(ScalarOptions))]
[JsonSerializable(typeof(AntiforgeryTokenSet))]
[JsonSerializable(typeof(ApiResponse))]
[JsonSerializable(typeof(AltchaChallenge))]
[JsonSerializable(typeof(ApplicationAdminEntity))]
[JsonSerializable(typeof(ApplicationUserEntity))]
[JsonSerializable(typeof(ApplicationTempUserEntity))]
[JsonSerializable(typeof(ReaderEntity))]
[JsonSerializable(typeof(CreateAdmin))]
[JsonSerializable(typeof(CreateUser))]
[JsonSerializable(typeof(CreateTempUser))]
[JsonSerializable(typeof(LoginUser))]
[JsonSerializable(typeof(GenerateUserToken))]
[JsonSerializable(typeof(ValidateUserExists))]
[JsonSerializable(typeof(ValidateTempUserExists))]
[JsonSerializable(typeof(ValidateUserNickNameTaken))]
[JsonSerializable(typeof(ReaderAccountTempUserCreated))]
[JsonSerializable(typeof(CheckEmailDomain))]
[JsonSerializable(typeof(SendGenericEmail))]
[JsonSerializable(typeof(SendTemplatedEmail))]
[JsonSerializable(typeof(TemplatedEmailSent))]
[JsonSerializable(typeof(RequestCreateReaderAccountDto))]
[JsonSerializable(typeof(RequestReaderAccountCreationResultDto))]
[JsonSerializable(typeof(ConfirmCreateReaderAccountDto))]
[JsonSerializable(typeof(ReaderAccountCreatedDto))]
[JsonSerializable(typeof(CheckNickNameNotTakenDto))]
[JsonSerializable(typeof(NickNameNotTakenCheckResultDto))]
[JsonSerializable(typeof(RequestAttestationOptionsDto))]
[JsonSerializable(typeof(RequestAssertionOptionsDto))]
[JsonSerializable(typeof(LoginByAssertionDto))]
[JsonSerializable(typeof(LoginByAssertionResultDto))]
[JsonSerializable(typeof(Fault<UserCreationResult>))]
[JsonSerializable(typeof(Fault<TempUserCreationResult>))]
[JsonSerializable(typeof(Fault<UserNickNameTakenValidationResult>))]
[JsonSerializable(typeof(Fault<EmailDomainChecked>))]
[JsonSerializable(typeof(Fault<ValidateUserExists>))]
[JsonSerializable(typeof(Fault<ValidateTempUserExists>))]
[JsonSerializable(typeof(Fault<ValidateAndCreateAttestation>))]
[JsonSerializable(typeof(Fault<ValidateAndCreateAssertion>))]
[JsonSerializable(typeof(Fault<CredentialCreateOptions>))]
[JsonSerializable(typeof(Fault<AssertionOptions>))]
[JsonSerializable(typeof(Fault<UserLoginResult>))]
[JsonSerializable(typeof(Fault<UserExistsValidationResult>))]
[JsonSerializable(typeof(Fault<TempUserExistsValidationResult>))]
[JsonSerializable(typeof(FaultEvent<UserCreationResult>))]
[JsonSerializable(typeof(FaultEvent<TempUserCreationResult>))]
[JsonSerializable(typeof(FaultEvent<UserNickNameTakenValidationResult>))]
[JsonSerializable(typeof(FaultEvent<EmailDomainChecked>))]
[JsonSerializable(typeof(FaultEvent<ValidateUserExists>))]
[JsonSerializable(typeof(FaultEvent<ValidateTempUserExists>))]
[JsonSerializable(typeof(FaultEvent<ValidateAndCreateAttestation>))]
[JsonSerializable(typeof(FaultEvent<ValidateAndCreateAssertion>))]
[JsonSerializable(typeof(FaultEvent<CredentialCreateOptions>))]
[JsonSerializable(typeof(FaultEvent<AssertionOptions>))]
[JsonSerializable(typeof(FaultEvent<UserLoginResult>))]
[JsonSerializable(typeof(FaultEvent<UserExistsValidationResult>))]
[JsonSerializable(typeof(FaultEvent<TempUserExistsValidationResult>))]
[JsonSerializable(typeof(DataResult<UserCreationResult>))]
[JsonSerializable(typeof(DataResult<TempUserCreationResult>))]
[JsonSerializable(typeof(DataResult<UserNickNameTakenValidationResult>))]
[JsonSerializable(typeof(DataResult<EmailDomainChecked>))]
[JsonSerializable(typeof(DataResult<ValidateUserExists>))]
[JsonSerializable(typeof(DataResult<ValidateTempUserExists>))]
[JsonSerializable(typeof(DataResult<ValidateAndCreateAttestation>))]
[JsonSerializable(typeof(DataResult<ValidateAndCreateAssertion>))]
[JsonSerializable(typeof(DataResult<CredentialCreateOptions>))]
[JsonSerializable(typeof(DataResult<AssertionOptions>))]
[JsonSerializable(typeof(DataResult<UserLoginResult>))]
[JsonSerializable(typeof(DataResult<UserExistsValidationResult>))]
[JsonSerializable(typeof(DataResult<TempUserExistsValidationResult>))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class CoreSerializationContext : JsonSerializerContext { }