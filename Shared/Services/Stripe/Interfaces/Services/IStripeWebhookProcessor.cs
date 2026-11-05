namespace snowcoreBlog.Backend.Stripe.Interfaces.Services;

/// <summary>What the webhook processor decided to do with a delivery.</summary>
public enum StripeWebhookOutcome
{
    /// <summary>Event verified, applied and persisted (or a no-op transition) — acknowledge with 200.</summary>
    Succeeded,

    /// <summary>Event id already processed — exactly-once ledger hit; acknowledge with 200.</summary>
    Duplicate,

    /// <summary>Event type intentionally not handled — acknowledge with 200 so Stripe stops retrying.</summary>
    NotApplicable,

    /// <summary>No local entity matched the event — acknowledge with 200 and log (never 4xx: Stripe disables endpoints on errors).</summary>
    EntityNotFound,

    /// <summary>Signature verification failed — reject with 400.</summary>
    InvalidSignature,

    /// <summary>WebhookSecret not configured — fail with 500 so the misconfiguration is loud.</summary>
    Misconfigured
}

/// <summary>Result of processing one Stripe webhook delivery.</summary>
public sealed record StripeWebhookResult(
    StripeWebhookOutcome Outcome,
    string? Detail = null)
{
    /// <summary>Domain events to publish on the bus AFTER the state change was persisted.</summary>
    public IReadOnlyList<object> EventsToPublish { get; init; } = Array.Empty<object>();

    public static readonly StripeWebhookResult Succeeded = new(StripeWebhookOutcome.Succeeded);
    public static StripeWebhookResult SucceededWithEvents(IReadOnlyList<object> events) => new(StripeWebhookOutcome.Succeeded) { EventsToPublish = events };
    public static StripeWebhookResult Duplicate() => new(StripeWebhookOutcome.Duplicate);
    public static StripeWebhookResult NotApplicable(string eventType) => new(StripeWebhookOutcome.NotApplicable, eventType);
    public static StripeWebhookResult EntityNotFound(string detail) => new(StripeWebhookOutcome.EntityNotFound, detail);
    public static StripeWebhookResult InvalidSignature(string detail) => new(StripeWebhookOutcome.InvalidSignature, detail);
    public static StripeWebhookResult Misconfigured(string detail) => new(StripeWebhookOutcome.Misconfigured, detail);
}

/// <summary>
/// Verifies, deduplicates and applies Stripe webhook deliveries to the local
/// donation/payment records. One call = one atomic transaction (Marten session)
/// covering both the entity transition and the webhook-ledger row.
/// </summary>
public interface IStripeWebhookProcessor
{
    Task<StripeWebhookResult> ProcessAsync(string json, string stripeSignatureHeader, CancellationToken token = default);
}
