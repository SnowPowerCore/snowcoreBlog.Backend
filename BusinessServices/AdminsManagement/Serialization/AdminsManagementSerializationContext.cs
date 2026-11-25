using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.AdminsManagement;

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
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.AdminDto))]
[JsonSerializable(typeof(List<PublicApi.BusinessObjects.Dto.AdminDto>))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.AdminInviteDto))]
[JsonSerializable(typeof(List<PublicApi.BusinessObjects.Dto.AdminInviteDto>))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.RequestAdminLoginLinkDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.ConfirmAdminLoginDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.CreateAdminInviteDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.AcceptAdminInviteDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.AdminInviteResolutionDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.RevokeAdminInviteResultDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.RequestAdminLoginLinkResultDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.DeleteAdminAccountResultDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.AdminLogoutResultDto))]
[JsonSerializable(typeof(IAM.Core.Contracts.AdminCreationResult))]
[JsonSerializable(typeof(IAM.Core.Contracts.AdminExistsValidationResult))]
[JsonSerializable(typeof(IAM.Core.Contracts.CreateAdmin))]
[JsonSerializable(typeof(Email.Core.Contracts.CheckEmailDomain))]
[JsonSerializable(typeof(Email.Core.Contracts.EmailDomainChecked))]
[JsonSerializable(typeof(MassTransit.Events.FaultEvent<IAM.Core.Contracts.CreateAdmin>))]
[JsonSerializable(typeof(MassTransit.Fault<IAM.Core.Contracts.CreateAdmin>))]
[JsonSerializable(typeof(PublicApi.Utilities.DataResult.DataResult<IAM.Core.Contracts.CreateAdmin>))]
[JsonSerializable(typeof(MassTransit.Events.FaultEvent<IAM.Core.Contracts.AdminCreationResult>))]
[JsonSerializable(typeof(MassTransit.Fault<IAM.Core.Contracts.AdminCreationResult>))]
[JsonSerializable(typeof(PublicApi.Utilities.DataResult.DataResult<IAM.Core.Contracts.AdminCreationResult>))]
[JsonSerializable(typeof(MassTransit.Events.FaultEvent<IAM.Core.Contracts.AdminExistsValidationResult>))]
[JsonSerializable(typeof(MassTransit.Fault<IAM.Core.Contracts.AdminExistsValidationResult>))]
[JsonSerializable(typeof(PublicApi.Utilities.DataResult.DataResult<IAM.Core.Contracts.AdminExistsValidationResult>))]
[JsonSerializable(typeof(MassTransit.Events.FaultEvent<Email.Core.Contracts.EmailDomainChecked>))]
[JsonSerializable(typeof(MassTransit.Fault<Email.Core.Contracts.EmailDomainChecked>))]
[JsonSerializable(typeof(PublicApi.Utilities.DataResult.DataResult<Email.Core.Contracts.EmailDomainChecked>))]
[JsonSerializable(typeof(Core.Entities.Admin.AdminEntity))]
[JsonSerializable(typeof(Core.Entities.Admin.AdminInviteEntity))]
[JsonSerializable(typeof(Core.Entities.Admin.AdminLoginTokenEntity))]
[JsonSerializable(typeof(Email.Core.Contracts.SendGenericEmail))]
[JsonSerializable(typeof(MassTransit.Fault<Email.Core.Contracts.SendGenericEmail>))]
[JsonSerializable(typeof(MassTransit.Events.FaultEvent<Email.Core.Contracts.SendGenericEmail>))]
[JsonSerializable(typeof(IAM.Core.Contracts.ValidateAdminExists))]
[JsonSerializable(typeof(MassTransit.Fault<IAM.Core.Contracts.ValidateAdminExists>))]
[JsonSerializable(typeof(MassTransit.Events.FaultEvent<IAM.Core.Contracts.ValidateAdminExists>))]
[JsonSerializable(typeof(PublicApi.Utilities.DataResult.DataResult<IAM.Core.Contracts.ValidateAdminExists>))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class AdminsManagementSerializationContext : JsonSerializerContext;
