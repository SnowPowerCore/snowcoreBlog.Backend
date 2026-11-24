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
