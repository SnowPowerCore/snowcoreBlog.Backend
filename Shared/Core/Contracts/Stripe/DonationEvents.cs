namespace snowcoreBlog.Backend.Core.Contracts.Stripe;

/// <summary>
/// Published on RabbitMQ when a donation was successfully paid. Consumed by services
/// that react to new donations (thank-you e-mails, public donators wall, analytics).
/// </summary>
public sealed record DonationPaid(
    Guid DonationId,
    string CheckoutSessionId,
    string? PaymentIntentId,
    long AmountMinor,
    string Currency,
    Guid? DonorUserId,
    string? DonorEmail,
    string? ReceiptUrl);

/// <summary>
/// Published on RabbitMQ when a donation checkout expired or the payment failed.
/// </summary>
public sealed record DonationFailed(
    Guid DonationId,
    string? PaymentIntentId,
    string? FailureReason);
