﻿using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fido2NetLib;
using FluentValidation.Results;
using Ixnas.AltchaNet;
using MassTransit;
using MassTransit.Events;
using Scalar.AspNetCore;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;
using snowcoreBlog.Backend.Core.Contracts;
using snowcoreBlog.Backend.Core.Entities.Reader;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.Backend.Infrastructure.Entities;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.Infrastructure;

/// <summary>
/// Defines the serialization context.
/// </summary>
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(List<byte>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(ScalarOptions))]
[JsonSerializable(typeof(List<ValidationFailure>))]
[JsonSerializable(typeof(ApiResponse))]
[JsonSerializable(typeof(AltchaChallenge))]
[JsonSerializable(typeof(AltchaResponse))]
[JsonSerializable(typeof(ApplicationAdminEntity))]
[JsonSerializable(typeof(ApplicationUserEntity))]
[JsonSerializable(typeof(ApplicationTempUserEntity))]
[JsonSerializable(typeof(Fido2PublicKeyCredentialEntity))]
[JsonSerializable(typeof(Fido2AuthenticatorTransportEntity))]
[JsonSerializable(typeof(Fido2DevicePublicKeyEntity))]
[JsonSerializable(typeof(ReaderEntity))]
[JsonSerializable(typeof(AltchaStoredChallengeEntity))]
[JsonSerializable(typeof(CreateAdmin))]
[JsonSerializable(typeof(CreateUser))]
[JsonSerializable(typeof(CreateTempUser))]
[JsonSerializable(typeof(LoginUser))]
[JsonSerializable(typeof(GetUserTokenPairWithPayload))]
[JsonSerializable(typeof(ValidateUserExists))]
[JsonSerializable(typeof(ValidateTempUserExists))]
[JsonSerializable(typeof(ValidateUserNickNameTaken))]
[JsonSerializable(typeof(ReaderAccountTempUserCreated))]
[JsonSerializable(typeof(ReaderAccountUserCreated))]
[JsonSerializable(typeof(ReaderAccountCreated))]
[JsonSerializable(typeof(CheckEmailDomain))]
[JsonSerializable(typeof(SendGenericEmail))]
[JsonSerializable(typeof(SendTemplatedEmail))]
[JsonSerializable(typeof(TemplatedEmailSent))]
[JsonSerializable(typeof(RequestCreateReaderAccountDto))]
[JsonSerializable(typeof(RequestReaderAccountCreationResultDto))]
[JsonSerializable(typeof(ConfirmCreateReaderAccountDto))]
[JsonSerializable(typeof(ReaderAccountCreatedDto))]
[JsonSerializable(typeof(AntiforgeryResultDto))]
[JsonSerializable(typeof(CheckNickNameNotTakenDto))]
[JsonSerializable(typeof(NickNameNotTakenCheckResultDto))]
[JsonSerializable(typeof(RequestAttestationOptionsDto))]
[JsonSerializable(typeof(RequestAssertionOptionsDto))]
[JsonSerializable(typeof(LoginByAssertionDto))]
[JsonSerializable(typeof(LoginByAssertionResultDto))]
[JsonSerializable(typeof(ReaderAccountUserLoggedIn))]
[JsonSerializable(typeof(AuthenticationStateDto))]
[JsonSerializable(typeof(Fault<UserCreationResult>))]
[JsonSerializable(typeof(Fault<TempUserCreationResult>))]
[JsonSerializable(typeof(Fault<UserNickNameTakenValidationResult>))]
[JsonSerializable(typeof(Fault<EmailDomainChecked>))]
[JsonSerializable(typeof(Fault<SendGenericEmail>))]
[JsonSerializable(typeof(Fault<SendTemplatedEmail>))]
[JsonSerializable(typeof(Fault<GenericEmailSent>))]
[JsonSerializable(typeof(Fault<TemplatedEmailSent>))]
[JsonSerializable(typeof(Fault<ValidateUserExists>))]
[JsonSerializable(typeof(Fault<ValidateTempUserExists>))]
[JsonSerializable(typeof(Fault<ValidateAndCreateAttestation>))]
[JsonSerializable(typeof(Fault<ValidateAndCreateAssertion>))]
[JsonSerializable(typeof(Fault<CredentialCreateOptions>))]
[JsonSerializable(typeof(Fault<AssertionOptions>))]
[JsonSerializable(typeof(Fault<LoginUser>))]
[JsonSerializable(typeof(Fault<UserLoginResult>))]
[JsonSerializable(typeof(Fault<ReaderAccountUserLoggedIn>))]
[JsonSerializable(typeof(Fault<UserExistsValidationResult>))]
[JsonSerializable(typeof(Fault<TempUserExistsValidationResult>))]
[JsonSerializable(typeof(Fault<GetUserTokenPairWithPayload>))]
[JsonSerializable(typeof(Fault<UserTokenPairWithPayloadGenerated>))]
[JsonSerializable(typeof(Fault<CreateAdmin>))]
[JsonSerializable(typeof(Fault<CreateUser>))]
[JsonSerializable(typeof(FaultEvent<UserCreationResult>))]
[JsonSerializable(typeof(FaultEvent<TempUserCreationResult>))]
[JsonSerializable(typeof(FaultEvent<UserNickNameTakenValidationResult>))]
[JsonSerializable(typeof(FaultEvent<EmailDomainChecked>))]
[JsonSerializable(typeof(FaultEvent<SendGenericEmail>))]
[JsonSerializable(typeof(FaultEvent<SendTemplatedEmail>))]
[JsonSerializable(typeof(FaultEvent<GenericEmailSent>))]
[JsonSerializable(typeof(FaultEvent<TemplatedEmailSent>))]
[JsonSerializable(typeof(FaultEvent<ValidateUserExists>))]
[JsonSerializable(typeof(FaultEvent<ValidateTempUserExists>))]
[JsonSerializable(typeof(FaultEvent<ValidateAndCreateAttestation>))]
[JsonSerializable(typeof(FaultEvent<ValidateAndCreateAssertion>))]
[JsonSerializable(typeof(FaultEvent<CredentialCreateOptions>))]
[JsonSerializable(typeof(FaultEvent<AssertionOptions>))]
[JsonSerializable(typeof(FaultEvent<LoginUser>))]
[JsonSerializable(typeof(FaultEvent<UserLoginResult>))]
[JsonSerializable(typeof(FaultEvent<ReaderAccountUserLoggedIn>))]
[JsonSerializable(typeof(FaultEvent<UserExistsValidationResult>))]
[JsonSerializable(typeof(FaultEvent<TempUserExistsValidationResult>))]
[JsonSerializable(typeof(FaultEvent<GetUserTokenPairWithPayload>))]
[JsonSerializable(typeof(FaultEvent<UserTokenPairWithPayloadGenerated>))]
[JsonSerializable(typeof(FaultEvent<CreateAdmin>))]
[JsonSerializable(typeof(FaultEvent<CreateUser>))]
[JsonSerializable(typeof(DataResult<UserCreationResult>))]
[JsonSerializable(typeof(DataResult<TempUserCreationResult>))]
[JsonSerializable(typeof(DataResult<UserNickNameTakenValidationResult>))]
[JsonSerializable(typeof(DataResult<EmailDomainChecked>))]
[JsonSerializable(typeof(DataResult<SendGenericEmail>))]
[JsonSerializable(typeof(DataResult<SendTemplatedEmail>))]
[JsonSerializable(typeof(DataResult<GenericEmailSent>))]
[JsonSerializable(typeof(DataResult<TemplatedEmailSent>))]
[JsonSerializable(typeof(DataResult<ValidateUserExists>))]
[JsonSerializable(typeof(DataResult<ValidateTempUserExists>))]
[JsonSerializable(typeof(DataResult<ValidateAndCreateAttestation>))]
[JsonSerializable(typeof(DataResult<ValidateAndCreateAssertion>))]
[JsonSerializable(typeof(DataResult<CredentialCreateOptions>))]
[JsonSerializable(typeof(DataResult<AssertionOptions>))]
[JsonSerializable(typeof(DataResult<LoginUser>))]
[JsonSerializable(typeof(DataResult<UserLoginResult>))]
[JsonSerializable(typeof(DataResult<ReaderAccountUserLoggedIn>))]
[JsonSerializable(typeof(DataResult<UserExistsValidationResult>))]
[JsonSerializable(typeof(DataResult<TempUserExistsValidationResult>))]
[JsonSerializable(typeof(DataResult<GetUserTokenPairWithPayload>))]
[JsonSerializable(typeof(DataResult<UserTokenPairWithPayloadGenerated>))]
[JsonSerializable(typeof(DataResult<CreateAdmin>))]
[JsonSerializable(typeof(DataResult<CreateUser>))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class CoreSerializationContext : JsonSerializerContext { }