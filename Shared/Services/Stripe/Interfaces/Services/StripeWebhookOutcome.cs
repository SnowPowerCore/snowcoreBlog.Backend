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
