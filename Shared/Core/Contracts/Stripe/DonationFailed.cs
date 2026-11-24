namespace snowcoreBlog.Backend.Core.Contracts.Stripe;

/// <summary>
/// Published on RabbitMQ when a donation checkout expired or the payment failed.
/// </summary>
public sealed record DonationFailed(
    Guid DonationId,
    string? PaymentIntentId,
    string? FailureReason);
