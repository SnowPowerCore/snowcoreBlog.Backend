using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.AdminsManagement;

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
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.AdminDto))]
[JsonSerializable(typeof(global::System.Collections.Generic.List<global::snowcoreBlog.PublicApi.BusinessObjects.Dto.AdminDto>))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.AdminInviteDto))]
[JsonSerializable(typeof(global::System.Collections.Generic.List<global::snowcoreBlog.PublicApi.BusinessObjects.Dto.AdminInviteDto>))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.RequestAdminLoginLinkDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.ConfirmAdminLoginDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.CreateAdminInviteDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.AcceptAdminInviteDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.AdminInviteResolutionDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.RevokeAdminInviteResultDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.RequestAdminLoginLinkResultDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.DeleteAdminAccountResultDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.AdminLogoutResultDto))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.IAM.Core.Contracts.AdminCreationResult))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.IAM.Core.Contracts.AdminExistsValidationResult))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.IAM.Core.Contracts.CreateAdmin))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.Email.Core.Contracts.CheckEmailDomain))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.Email.Core.Contracts.EmailDomainChecked))]
[JsonSerializable(typeof(global::MassTransit.Events.FaultEvent<global::snowcoreBlog.Backend.IAM.Core.Contracts.CreateAdmin>))]
[JsonSerializable(typeof(global::MassTransit.Fault<global::snowcoreBlog.Backend.IAM.Core.Contracts.CreateAdmin>))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.Utilities.DataResult.DataResult<global::snowcoreBlog.Backend.IAM.Core.Contracts.CreateAdmin>))]
[JsonSerializable(typeof(global::MassTransit.Events.FaultEvent<global::snowcoreBlog.Backend.IAM.Core.Contracts.AdminCreationResult>))]
[JsonSerializable(typeof(global::MassTransit.Fault<global::snowcoreBlog.Backend.IAM.Core.Contracts.AdminCreationResult>))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.Utilities.DataResult.DataResult<global::snowcoreBlog.Backend.IAM.Core.Contracts.AdminCreationResult>))]
[JsonSerializable(typeof(global::MassTransit.Events.FaultEvent<global::snowcoreBlog.Backend.IAM.Core.Contracts.AdminExistsValidationResult>))]
[JsonSerializable(typeof(global::MassTransit.Fault<global::snowcoreBlog.Backend.IAM.Core.Contracts.AdminExistsValidationResult>))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.Utilities.DataResult.DataResult<global::snowcoreBlog.Backend.IAM.Core.Contracts.AdminExistsValidationResult>))]
[JsonSerializable(typeof(global::MassTransit.Events.FaultEvent<global::snowcoreBlog.Backend.Email.Core.Contracts.EmailDomainChecked>))]
[JsonSerializable(typeof(global::MassTransit.Fault<global::snowcoreBlog.Backend.Email.Core.Contracts.EmailDomainChecked>))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.Utilities.DataResult.DataResult<global::snowcoreBlog.Backend.Email.Core.Contracts.EmailDomainChecked>))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class AdminsManagementSerializationContext : JsonSerializerContext;
