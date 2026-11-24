using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.ServiceNotifications;

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
[JsonSerializable(typeof(global::snowcoreBlog.Backend.ServiceNotifications.Core.Entities.Notification.NotificationEntity))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.ServiceNotifications.Core.Entities.Notification.NotificationType))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.NotificationDto))]
[JsonSerializable(typeof(global::System.Collections.Generic.List<global::snowcoreBlog.PublicApi.BusinessObjects.Dto.NotificationDto>))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.RequestUserNotifications))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.UserNotificationsResponse))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.NotificationTypeDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.CreateNotificationDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.CreateNotificationResultDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.UpdateNotificationDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.DeleteNotificationDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.DeleteNotificationResultDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.GetActiveNotificationsDto))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class ServiceNotificationsSerializationContext : JsonSerializerContext;
