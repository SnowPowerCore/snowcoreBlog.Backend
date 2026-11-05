using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core.Entities.Stripe;

/// <summary>
/// A one-time donation processed through a Stripe Checkout Session
/// (mode <c>payment</c>, submit type <c>donate</c>). Created in <see cref="StripePaymentStatus.Pending"/>
/// before the session URL is returned to the client; the only writer of later
/// transitions is the webhook endpoint.
/// </summary>
public record DonationEntity : BaseEntity
{
    /// <summary>Stripe Checkout Session id (cs_...).</summary>
    public required string CheckoutSessionId { get; init; }

    /// <summary>Hosted Checkout URL the client is redirected to.</summary>
    public required string CheckoutUrl { get; init; }

    /// <summary>Stripe PaymentIntent id (pi_...), filled from webhooks.</summary>
    public string? PaymentIntentId { get; init; }

    /// <summary>Stripe Customer id attached to the payment, if any.</summary>
    public string? StripeCustomerId { get; init; }

    /// <summary>Application-side account id of the donor, when known.</summary>
    public Guid? DonorUserId { get; init; }

    /// <summary>Donor e-mail (prefilled or captured by Checkout).</summary>
    public string? DonorEmail { get; init; }

    /// <summary>Donation amount in the currency's minor units (cents).</summary>
    public required long AmountMinor { get; init; }

    /// <summary>Three-letter ISO currency code, lowercase.</summary>
    public required string Currency { get; init; }

    /// <summary>Lifecycle status (see <see cref="StripePaymentStatus"/>).</summary>
    public StripePaymentStatus Status { get; init; } = StripePaymentStatus.Pending;

    /// <summary>Optional public thank-you note written by the donor.</summary>
    public string? Message { get; init; }

    /// <summary>Stripe-hosted receipt URL, filled from the charge webhook.</summary>
    public string? ReceiptUrl { get; init; }

    /// <summary>Human-readable failure reason from the failed payment webhook.</summary>
    public string? FailureReason { get; init; }

    /// <summary>Free-form metadata persisted alongside the payment.</summary>
    public Dictionary<string, string>? Metadata { get; init; }

    /// <summary>UTC timestamp of record creation.</summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>UTC timestamp when the payment succeeded (null until then).</summary>
    public DateTime? PaidAt { get; init; }
}
