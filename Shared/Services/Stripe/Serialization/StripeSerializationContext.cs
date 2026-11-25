using System.Text.Json;
using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.Stripe;

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
[JsonSerializable(typeof(global::snowcoreBlog.Backend.Stripe.Core.Entities.Stripe.DonationEntity))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.Stripe.Core.Entities.Stripe.PaymentEntity))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.Stripe.Core.Entities.Stripe.StripeCustomerEntity))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.Stripe.Core.Entities.Stripe.StripeWebhookEventEntity))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.Stripe.Core.Entities.Stripe.StripePaymentStatus))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.Stripe.Core.Contracts.Stripe.EnsureStripeCustomer))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.Stripe.Core.Contracts.Stripe.StripeCustomerEnsured))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.Stripe.Core.Contracts.Stripe.DonationPaid))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.Stripe.Core.Contracts.Stripe.DonationFailed))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.Stripe.Core.Contracts.Stripe.PaymentPaid))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.Stripe.Core.Contracts.Stripe.PaymentFailed))]
[JsonSerializable(typeof(global::snowcoreBlog.Backend.Stripe.Core.Contracts.Stripe.PaymentRefunded))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.CreateDonationCheckoutDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.DonationCheckoutResultDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.CreateGoodsCheckoutDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.GoodsCheckoutResultDto))]
[JsonSerializable(typeof(global::MassTransit.Events.FaultEvent<global::snowcoreBlog.PublicApi.BusinessObjects.Dto.DonationCheckoutResultDto>))]
[JsonSerializable(typeof(global::MassTransit.Fault<global::snowcoreBlog.PublicApi.BusinessObjects.Dto.DonationCheckoutResultDto>))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.Utilities.DataResult.DataResult<global::snowcoreBlog.PublicApi.BusinessObjects.Dto.DonationCheckoutResultDto>))]
[JsonSerializable(typeof(global::MassTransit.Events.FaultEvent<global::snowcoreBlog.PublicApi.BusinessObjects.Dto.GoodsCheckoutResultDto>))]
[JsonSerializable(typeof(global::MassTransit.Fault<global::snowcoreBlog.PublicApi.BusinessObjects.Dto.GoodsCheckoutResultDto>))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.Utilities.DataResult.DataResult<global::snowcoreBlog.PublicApi.BusinessObjects.Dto.GoodsCheckoutResultDto>))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.DonationDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.PaymentDto))]
[JsonSerializable(typeof(global::snowcoreBlog.PublicApi.BusinessObjects.Dto.StripePaymentStatusDto))]
[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    UseStringEnumConverter = true,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class StripeSerializationContext : JsonSerializerContext;
