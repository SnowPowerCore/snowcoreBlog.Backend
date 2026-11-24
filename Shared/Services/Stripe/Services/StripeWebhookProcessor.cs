using System.Text.Json;
using Marten;
using Microsoft.Extensions.Options;
using Stripe;
using snowcoreBlog.Backend.Stripe.Core.Contracts.Stripe;
using snowcoreBlog.Backend.Stripe.Core.Entities.Stripe;
using snowcoreBlog.Backend.Stripe.Interfaces.Services;
using snowcoreBlog.Backend.Stripe.Options;

namespace snowcoreBlog.Backend.Stripe.Services;

/// <inheritdoc />
public class StripeWebhookProcessor(
    IOptions<StripeOptions> options,
    IDocumentSession session,
    ILogger<StripeWebhookProcessor> logger) : IStripeWebhookProcessor
{
    // Stripe event type strings (public API contract; see EventTypes in Stripe.net).
    private const string CheckoutSessionCompleted = "checkout.session.completed";
    private const string CheckoutSessionAsyncPaymentSucceeded = "checkout.session.async_payment_succeeded";
    private const string CheckoutSessionAsyncPaymentFailed = "checkout.session.async_payment_failed";
    private const string CheckoutSessionExpired = "checkout.session.expired";
    private const string PaymentIntentSucceeded = "payment_intent.succeeded";
    private const string PaymentIntentPaymentFailed = "payment_intent.payment_failed";
    private const string ChargeSucceeded = "charge.succeeded";
    private const string ChargeRefunded = "charge.refunded";

    // Metadata keys this service stamps on Checkout Sessions / PaymentIntents so
    // every webhook payload carries a way back to the local record.
    public const string MetadataRecordIdKey = "payment_record_id";
    public const string MetadataRecordKindKey = "payment_record_kind";
    public const string KindDonation = "donation";
    public const string KindPayment = "payment";

    public async Task<StripeWebhookResult> ProcessAsync(string json, string stripeSignatureHeader, CancellationToken token = default)
    {
        var webhookSecret = options.Value.WebhookSecret;
        if (string.IsNullOrWhiteSpace(webhookSecret))
        {
            logger.LogError("Stripe webhook received but Stripe:WebhookSecret is not configured вЂ” rejecting with 500.");
            return StripeWebhookResult.Misconfigured("Stripe:WebhookSecret is not configured.");
        }

        // 1. Authenticity: HMAC verification against the raw payload. Anything failing
        //    this is a forged/untrusted request and never touches the database.
        global::Stripe.Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, stripeSignatureHeader, webhookSecret);
        }
        catch (StripeException ex)
        {
            logger.LogWarning("Stripe webhook signature verification failed: {Message}", ex.Message);
            return StripeWebhookResult.InvalidSignature(ex.Message);
        }

        // 2. Exactly-once: Stripe delivers at-least-once; the unique EventId ledger
        //    (arbitrated again by the unique index inside the transaction) makes
        //    replays cheap no-ops.
        if (await session.Query<StripeWebhookEventEntity>()
                .AnyAsync(e => e.EventId == stripeEvent.Id, token))
        {
            logger.LogInformation("Stripe event {EventId} already processed вЂ” acknowledging duplicate.", stripeEvent.Id);
            return StripeWebhookResult.Duplicate();
        }

        // 3. Route on the raw payload. Parsing is decoupled from Stripe.net's typed
        //    event models on purpose вЂ” the JSON shape of data.object is Stripe's
        //    stable public API and keeps this processor independent from SDK churn.
        using var document = JsonDocument.Parse(json);
        var dataObject = document.RootElement.GetProperty("data").GetProperty("object");
        var metadata = GetMetadataDictionary(dataObject);

        return stripeEvent.Type switch
        {
            CheckoutSessionCompleted or CheckoutSessionAsyncPaymentSucceeded
                => await ApplySessionPaidAsync(stripeEvent, dataObject, metadata, token),
            CheckoutSessionExpired or CheckoutSessionAsyncPaymentFailed
                => await ApplySessionCanceledAsync(stripeEvent, dataObject, metadata, token),
            PaymentIntentSucceeded
                => await ApplyIntentTransitionAsync(stripeEvent, dataObject, metadata, StripePaymentStatus.Succeeded, token),
            PaymentIntentPaymentFailed
                => await ApplyIntentTransitionAsync(stripeEvent, dataObject, metadata, StripePaymentStatus.Failed, token),
            ChargeSucceeded
                => await ApplyReceiptAsync(stripeEvent, dataObject, token),
            ChargeRefunded
                => await ApplyRefundAsync(stripeEvent, dataObject, metadata, token),
            _ => StripeWebhookResult.NotApplicable(stripeEvent.Type)
        };
    }

    // --- Session paid ------------------------------------------------------------

    private async Task<StripeWebhookResult> ApplySessionPaidAsync(
        global::Stripe.Event stripeEvent, JsonElement dataObject, Dictionary<string, string>? metadata, CancellationToken token)
    {
        var (recordId, kind) = ResolveRecordReference(metadata, dataObject);
        if (recordId is null)
        {
            return StripeWebhookResult.EntityNotFound($"No {MetadataRecordIdKey} metadata on paid session {TryGetString(dataObject, "id")}.");
        }

        var now = DateTime.UtcNow;

        if (kind == KindDonation)
        {
            var donation = await session.LoadAsync<DonationEntity>(recordId.Value, token);
            if (donation is null)
            {
                return StripeWebhookResult.EntityNotFound($"Donation {recordId} not found for paid session.");
            }

            IReadOnlyList<object> events = [];
            if (donation.Status is StripePaymentStatus.Pending or StripePaymentStatus.Processing)
            {
                donation = donation with
                {
                    Status = StripePaymentStatus.Succeeded,
                    PaymentIntentId = TryGetString(dataObject, "payment_intent") ?? donation.PaymentIntentId,
                    StripeCustomerId = TryGetString(dataObject, "customer") ?? donation.StripeCustomerId,
                    DonorEmail = TryGetString(dataObject, "customer_email") ?? donation.DonorEmail,
                    PaidAt = now
                };
                events = [ToPaidEvent(donation)];
            }
            else
            {
                logger.LogInformation("Donation {DonationId} already in status {Status} вЂ” paid session event is a no-op.", donation.Id, donation.Status);
            }

            return await PersistAsync(stripeEvent, donation.Id, kind, donation, events, token);
        }

        var payment = await session.LoadAsync<PaymentEntity>(recordId.Value, token);
        if (payment is null)
        {
            return StripeWebhookResult.EntityNotFound($"Payment {recordId} not found for paid session.");
        }

        IReadOnlyList<object> paymentEvents = [];
        if (payment.Status is StripePaymentStatus.Pending or StripePaymentStatus.Processing)
        {
            payment = payment with
            {
                Status = StripePaymentStatus.Succeeded,
                PaymentIntentId = TryGetString(dataObject, "payment_intent") ?? payment.PaymentIntentId,
                StripeCustomerId = TryGetString(dataObject, "customer") ?? payment.StripeCustomerId,
                BuyerEmail = TryGetString(dataObject, "customer_email") ?? payment.BuyerEmail,
                PaidAt = now
            };
            paymentEvents = [ToPaidEvent(payment)];
        }
        else
        {
            logger.LogInformation("Payment {PaymentId} already in status {Status} вЂ” paid session event is a no-op.", payment.Id, payment.Status);
        }

        return await PersistAsync(stripeEvent, payment.Id, kind, payment, paymentEvents, token);
    }

    // --- Session expired / async failure -------------------------------------------

    private async Task<StripeWebhookResult> ApplySessionCanceledAsync(
        global::Stripe.Event stripeEvent, JsonElement dataObject, Dictionary<string, string>? metadata, CancellationToken token)
    {
        var (recordId, kind) = ResolveRecordReference(metadata, dataObject);
        if (recordId is null)
        {
            return StripeWebhookResult.EntityNotFound($"No {MetadataRecordIdKey} metadata on expired session.");
        }

        if (kind == KindDonation)
        {
            var donation = await session.LoadAsync<DonationEntity>(recordId.Value, token);
            if (donation is null)
            {
                return StripeWebhookResult.EntityNotFound($"Donation {recordId} not found for expired session.");
            }

            IReadOnlyList<object> events = [];
            if (donation.Status == StripePaymentStatus.Pending)
            {
                donation = donation with { Status = StripePaymentStatus.Canceled, FailureReason = "Checkout Session expired." };
                events = [new DonationFailed(donation.Id, donation.PaymentIntentId, "Checkout session expired.")];
            }

            return await PersistAsync(stripeEvent, donation.Id, kind, donation, events, token);
        }

        var payment = await session.LoadAsync<PaymentEntity>(recordId.Value, token);
        if (payment is null)
        {
            return StripeWebhookResult.EntityNotFound($"Payment {recordId} not found for expired session.");
        }

        IReadOnlyList<object> paymentEvents = [];
        if (payment.Status == StripePaymentStatus.Pending)
        {
            payment = payment with { Status = StripePaymentStatus.Canceled, FailureReason = "Checkout Session expired." };
            paymentEvents = [new PaymentFailed(payment.Id, payment.ReferenceId, payment.PaymentIntentId, "Checkout session expired.")];
        }

        return await PersistAsync(stripeEvent, payment.Id, kind, payment, paymentEvents, token);
    }

    // --- PaymentIntent transitions (async settlement + explicit failure) -----------

    private async Task<StripeWebhookResult> ApplyIntentTransitionAsync(
        global::Stripe.Event stripeEvent, JsonElement dataObject, Dictionary<string, string>? metadata,
        StripePaymentStatus targetStatus, CancellationToken token)
    {
        var (recordId, kind) = ResolveRecordReference(metadata, dataObject);
        if (recordId is null)
        {
            return StripeWebhookResult.EntityNotFound($"No {MetadataRecordIdKey} metadata on payment intent event {stripeEvent.Type}.");
        }

        var intentId = TryGetString(dataObject, "id");
        var failureReason = targetStatus == StripePaymentStatus.Failed
            ? TryGetString(dataObject, "last_payment_error", "message") ?? "Payment failed."
            : null;
        var now = DateTime.UtcNow;

        if (kind == KindDonation)
        {
            var donation = await session.LoadAsync<DonationEntity>(recordId.Value, token);
            if (donation is null)
            {
                return StripeWebhookResult.EntityNotFound($"Donation {recordId} not found for intent event {stripeEvent.Type}.");
            }

            IReadOnlyList<object> events = [];
            if (donation.Status is StripePaymentStatus.Pending or StripePaymentStatus.Processing)
            {
                donation = donation with
                {
                    Status = targetStatus,
                    PaymentIntentId = intentId ?? donation.PaymentIntentId,
                    FailureReason = failureReason ?? donation.FailureReason,
                    PaidAt = targetStatus == StripePaymentStatus.Succeeded ? now : donation.PaidAt
                };
                events = targetStatus == StripePaymentStatus.Succeeded
                    ? [ToPaidEvent(donation)]
                    : [new DonationFailed(donation.Id, donation.PaymentIntentId, failureReason)];
            }

            return await PersistAsync(stripeEvent, donation.Id, kind, donation, events, token);
        }

        var payment = await session.LoadAsync<PaymentEntity>(recordId.Value, token);
        if (payment is null)
        {
            return StripeWebhookResult.EntityNotFound($"Payment {recordId} not found for intent event {stripeEvent.Type}.");
        }

        IReadOnlyList<object> paymentEvents = [];
        if (payment.Status is StripePaymentStatus.Pending or StripePaymentStatus.Processing)
        {
            payment = payment with
            {
                Status = targetStatus,
                PaymentIntentId = intentId ?? payment.PaymentIntentId,
                FailureReason = failureReason ?? payment.FailureReason,
                PaidAt = targetStatus == StripePaymentStatus.Succeeded ? now : payment.PaidAt
            };
            paymentEvents = targetStatus == StripePaymentStatus.Succeeded
                ? [ToPaidEvent(payment)]
                : [new PaymentFailed(payment.Id, payment.ReferenceId, payment.PaymentIntentId, failureReason)];
        }

        return await PersistAsync(stripeEvent, payment.Id, kind, payment, paymentEvents, token);
    }

    // --- Receipts (charge.succeeded) -------------------------------------------------

    private async Task<StripeWebhookResult> ApplyReceiptAsync(
        global::Stripe.Event stripeEvent, JsonElement dataObject, CancellationToken token)
    {
        var intentId = TryGetString(dataObject, "payment_intent");
        var receiptUrl = TryGetString(dataObject, "receipt_url");
        if (intentId is null || receiptUrl is null)
        {
            return StripeWebhookResult.NotApplicable(stripeEvent.Type);
        }

        var donation = await session.Query<DonationEntity>()
            .FirstOrDefaultAsync(d => d.PaymentIntentId == intentId, token);
        if (donation is not null)
        {
            if (donation.ReceiptUrl is not null)
            {
                return StripeWebhookResult.Succeeded;
            }

            return await PersistAsync(stripeEvent, donation.Id, KindDonation, donation with { ReceiptUrl = receiptUrl }, [], token);
        }

        var payment = await session.Query<PaymentEntity>()
            .FirstOrDefaultAsync(p => p.PaymentIntentId == intentId, token);
        if (payment is not null)
        {
            if (payment.ReceiptUrl is not null)
            {
                return StripeWebhookResult.Succeeded;
            }

            return await PersistAsync(stripeEvent, payment.Id, KindPayment, payment with { ReceiptUrl = receiptUrl }, [], token);
        }

        return StripeWebhookResult.EntityNotFound($"No record with payment intent {intentId} for receipt enrichment.");
    }

    // --- Refunds (charge.refunded) ----------------------------------------------------

    private async Task<StripeWebhookResult> ApplyRefundAsync(
        global::Stripe.Event stripeEvent, JsonElement dataObject, Dictionary<string, string>? metadata, CancellationToken token)
    {
        var (recordId, kind) = ResolveRecordReference(metadata, dataObject);
        if (recordId is null)
        {
            return StripeWebhookResult.EntityNotFound($"No {MetadataRecordIdKey} metadata on refunded charge.");
        }

        var amountRefunded = TryGetInt64(dataObject, "amount_refunded") ?? 0;
        var intentId = TryGetString(dataObject, "payment_intent");

        if (kind == KindDonation)
        {
            var donation = await session.LoadAsync<DonationEntity>(recordId.Value, token);
            if (donation is null)
            {
                return StripeWebhookResult.EntityNotFound($"Donation {recordId} not found for refund.");
            }

            IReadOnlyList<object> events = [];
            if (donation.Status == StripePaymentStatus.Succeeded)
            {
                // Donation refunds currently carry no dedicated event contract; the
                // status flip is recorded and logged for reconciliation.
                donation = donation with
                {
                    Status = StripePaymentStatus.Refunded,
                    PaymentIntentId = intentId ?? donation.PaymentIntentId,
                    FailureReason = "Donation refunded."
                };
                logger.LogInformation("Donation {DonationId} refunded ({AmountMinor} {Currency}).", donation.Id, amountRefunded, donation.Currency);
            }

            return await PersistAsync(stripeEvent, donation.Id, kind, donation, events, token);
        }

        var payment = await session.LoadAsync<PaymentEntity>(recordId.Value, token);
        if (payment is null)
        {
            return StripeWebhookResult.EntityNotFound($"Payment {recordId} not found for refund.");
        }

        IReadOnlyList<object> paymentEvents = [];
        if (payment.Status == StripePaymentStatus.Succeeded)
        {
            payment = payment with
            {
                Status = StripePaymentStatus.Refunded,
                PaymentIntentId = intentId ?? payment.PaymentIntentId
            };
            paymentEvents = [new PaymentRefunded(payment.Id, payment.ReferenceId, payment.PaymentIntentId, amountRefunded, payment.Currency, "Refunded via Stripe.")];
        }

        return await PersistAsync(stripeEvent, payment.Id, kind, payment, paymentEvents, token);
    }

    // --- Persistence (single atomic transaction) -----------------------------------------

    /// <summary>
    /// Stores the entity update and the webhook-ledger row in ONE SaveChangesAsync вЂ”
    /// the scoped IDocumentSession is shared with all repositories, so this is a single
    /// Postgres transaction: either both land or neither does. The unique EventId index
    /// also arbitrates concurrent replays (second insert fails в†’ treated as duplicate).
    /// Domain events are returned to the caller and published only AFTER the commit.
    /// </summary>
    private async Task<StripeWebhookResult> PersistAsync(
        global::Stripe.Event stripeEvent, Guid recordId, string kind,
        object updatedEntity, IReadOnlyList<object> eventsToPublish, CancellationToken token)
    {
        session.Store(updatedEntity);
        session.Store(new StripeWebhookEventEntity
        {
            Id = Guid.NewGuid(),
            EventId = stripeEvent.Id,
            EventType = stripeEvent.Type,
            RelatedEntityType = kind,
            RelatedEntityId = recordId,
            ProcessedAt = DateTime.UtcNow
        });

        await session.SaveChangesAsync(token);

        return eventsToPublish.Count > 0
            ? StripeWebhookResult.SucceededWithEvents(eventsToPublish)
            : StripeWebhookResult.Succeeded;
    }

    // --- Event mapping helpers -------------------------------------------------------------

    private static DonationPaid ToPaidEvent(DonationEntity donation) =>
        new(donation.Id, donation.CheckoutSessionId, donation.PaymentIntentId, donation.AmountMinor, donation.Currency, donation.DonorUserId, donation.DonorEmail, donation.ReceiptUrl);

    private static PaymentPaid ToPaidEvent(PaymentEntity payment) =>
        new(payment.Id, payment.ReferenceId, payment.CheckoutSessionId, payment.PaymentIntentId, payment.AmountMinor, payment.Currency, payment.BuyerUserId, payment.BuyerEmail, payment.ReceiptUrl);

    // --- Raw payload helpers -------------------------------------------------------------

    private static (Guid? RecordId, string Kind) ResolveRecordReference(Dictionary<string, string>? metadata, JsonElement dataObject)
    {
        var kind = metadata is not null && metadata.TryGetValue(MetadataRecordKindKey, out var k) ? k : KindDonation;
        if (metadata is not null && metadata.TryGetValue(MetadataRecordIdKey, out var idValue) && Guid.TryParse(idValue, out var id))
        {
            return (id, kind);
        }

        // Fallback: Checkout stamps client_reference_id with the record id.
        var clientReferenceId = TryGetString(dataObject, "client_reference_id");
        return Guid.TryParse(clientReferenceId, out var fromRef) ? (fromRef, kind) : (null, kind);
    }

    private static Dictionary<string, string>? GetMetadataDictionary(JsonElement dataObject)
    {
        if (!dataObject.TryGetProperty("metadata", out var metadataElement) ||
            metadataElement.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var property in metadataElement.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.String)
            {
                result[property.Name] = property.Value.GetString()!;
            }
        }

        return result.Count > 0 ? result : null;
    }

    private static string? TryGetString(JsonElement element, params string[] path)
    {
        var current = element;
        foreach (var segment in path)
        {
            if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty(segment, out current))
            {
                return null;
            }
        }

        return current.ValueKind == JsonValueKind.String ? current.GetString() : null;
    }

    private static long? TryGetInt64(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.Number
            ? value.GetInt64()
            : null;
}
