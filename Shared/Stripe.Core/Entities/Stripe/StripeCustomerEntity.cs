using snowcoreBlog.Backend.Shared.Core.Base;

namespace snowcoreBlog.Backend.Stripe.Core.Entities.Stripe;

/// <summary>
/// Maps an application account (reader/author/admin) to the Stripe Customer object
/// created for it, so every checkout started by that account attaches to the same
/// Stripe customer (payment history, receipts, saved payment methods).
/// </summary>
public record StripeCustomerEntity : BaseEntity
{
    /// <summary>Application-side account id. Null for guest (anonymous) customers.</summary>
    public Guid? UserId { get; init; }

    /// <summary>Customer e-mail address as known by the application.</summary>
    public required string Email { get; init; }

    /// <summary>Optional display name handed to Stripe.</summary>
    public string? Name { get; init; }

    /// <summary>The Stripe Customer object id (cus_...).</summary>
    public required string StripeCustomerId { get; init; }

    /// <summary>UTC timestamp when the mapping was created.</summary>
    public DateTime CreatedAt { get; init; }
}
