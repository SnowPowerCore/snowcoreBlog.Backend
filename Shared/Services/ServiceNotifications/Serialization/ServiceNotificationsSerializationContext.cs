using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.ServiceNotifications;

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
[JsonSerializable(typeof(Core.Entities.Notification.NotificationEntity))]
[JsonSerializable(typeof(Core.Entities.Notification.NotificationType))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.NotificationDto))]
[JsonSerializable(typeof(List<PublicApi.BusinessObjects.Dto.NotificationDto>))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.RequestUserNotifications))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.UserNotificationsResponse))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.NotificationTypeDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.CreateNotificationDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.CreateNotificationResultDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.UpdateNotificationDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.DeleteNotificationDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.DeleteNotificationResultDto))]
[JsonSerializable(typeof(PublicApi.BusinessObjects.Dto.GetActiveNotificationsDto))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class ServiceNotificationsSerializationContext : JsonSerializerContext;
