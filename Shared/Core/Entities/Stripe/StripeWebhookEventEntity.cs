using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.Core.Entities.Stripe;

/// <summary>
/// Deduplication ledger for Stripe webhook deliveries. Stripe guarantees at-least-once
/// delivery; the unique <see cref="EventId"/> index makes processing exactly-once —
/// replays of an already-handled event are acknowledged with 200 without re-applying.
/// </summary>
public record StripeWebhookEventEntity : BaseEntity
{
    /// <summary>Stripe event id (evt_...) — globally unique per event.</summary>
    public required string EventId { get; init; }

    /// <summary>Stripe event type string (e.g. checkout.session.completed).</summary>
    public required string EventType { get; init; }

    /// <summary>Entity type this event was applied to (donation/payment), when any.</summary>
    public string? RelatedEntityType { get; init; }

    /// <summary>Entity id this event was applied to, when any.</summary>
    public Guid? RelatedEntityId { get; init; }

    /// <summary>UTC timestamp the event was received and processed.</summary>
    public DateTime ProcessedAt { get; init; }
}
