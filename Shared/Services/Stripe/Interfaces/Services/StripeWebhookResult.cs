namespace snowcoreBlog.Backend.Stripe.Interfaces.Services;

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
