using snowcoreBlog.Backend.Stripe.Core.Entities.Stripe;

namespace snowcoreBlog.Backend.Stripe.Interfaces.Services;

/// <summary>
/// Guarantees a Stripe Customer object exists for an application account and keeps
/// the local mapping in sync. Used by checkouts (attach the payer to a customer)
/// and by the cross-service <c>EnsureStripeCustomer</c> request/response consumer.
/// </summary>
public interface IStripeCustomerService
{
    /// <summary>
    /// Returns the existing mapping or creates the Stripe customer and the mapping.
    /// Safe to call repeatedly вЂ” the lookup happens before any Stripe API call.
    /// </summary>
    Task<StripeCustomerEntity> EnsureAsync(Guid? userId, string email, string? name, CancellationToken token = default);

    /// <summary>Looks up the mapping for an application account without creating anything.</summary>
    Task<StripeCustomerEntity?> GetByUserIdAsync(Guid userId, CancellationToken token = default);
}
