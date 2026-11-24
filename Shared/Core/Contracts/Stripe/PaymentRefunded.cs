namespace snowcoreBlog.Backend.Core.Contracts.Stripe;

/// <summary>
/// Published on RabbitMQ when a previously successful payment was refunded.
/// Consumers owning the order should revoke/adjust the fulfilment.
/// </summary>
public sealed record PaymentRefunded(
    Guid PaymentId,
    string ReferenceId,
    string? PaymentIntentId,
    long AmountRefundedMinor,
    string Currency,
    string? Reason);
