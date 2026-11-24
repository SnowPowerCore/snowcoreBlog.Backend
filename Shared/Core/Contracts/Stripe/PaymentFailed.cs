namespace snowcoreBlog.Backend.Core.Contracts.Stripe;

/// <summary>
/// Published on RabbitMQ when a goods payment failed or its checkout expired.
/// </summary>
public sealed record PaymentFailed(
    Guid PaymentId,
    string ReferenceId,
    string? PaymentIntentId,
    string? FailureReason);
