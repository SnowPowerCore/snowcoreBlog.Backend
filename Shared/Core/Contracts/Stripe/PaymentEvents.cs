namespace snowcoreBlog.Backend.Core.Contracts.Stripe;

/// <summary>
/// Published on RabbitMQ when a payment for goods/services succeeded. Consumed by the
/// service owning the order (identified via <see cref="ReferenceId"/>) to fulfil it.
/// </summary>
public sealed record PaymentPaid(
    Guid PaymentId,
    string ReferenceId,
    string CheckoutSessionId,
    string? PaymentIntentId,
    long AmountMinor,
    string Currency,
    Guid? BuyerUserId,
    string? BuyerEmail,
    string? ReceiptUrl);

/// <summary>
/// Published on RabbitMQ when a goods payment failed or its checkout expired.
/// </summary>
public sealed record PaymentFailed(
    Guid PaymentId,
    string ReferenceId,
    string? PaymentIntentId,
    string? FailureReason);

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
