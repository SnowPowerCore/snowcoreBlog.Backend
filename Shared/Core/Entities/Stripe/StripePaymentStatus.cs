namespace snowcoreBlog.Backend.Core.Entities.Stripe;

/// <summary>
/// Lifecycle of a Stripe-backed payment (donation or goods payment) as tracked by
/// this service. Transitions are driven exclusively by Stripe webhook events —
/// never by the client-facing endpoints — so the status always mirrors reality.
/// </summary>
public enum StripePaymentStatus
{
    /// <summary>Checkout session created, waiting for the payer.</summary>
    Pending = 0,

    /// <summary>Payment submitted, asynchronous confirmation pending (e.g. bank transfers).</summary>
    Processing = 1,

    /// <summary>Payment confirmed by Stripe (checkout.session.completed / payment_intent.succeeded).</summary>
    Succeeded = 2,

    /// <summary>Payment failed or expired without completion.</summary>
    Failed = 3,

    /// <summary>Payer abandoned the checkout session (checkout.session.expired).</summary>
    Canceled = 4,

    /// <summary>Payment was (fully) refunded via Stripe (charge.refunded).</summary>
    Refunded = 5
}
