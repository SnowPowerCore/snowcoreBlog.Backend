using snowcoreBlog.Backend.Stripe.Core.Entities.Stripe;
using snowcoreBlog.Backend.Shared.Core.Interfaces.Repositories;

namespace snowcoreBlog.Backend.Stripe.Interfaces.Repositories.Marten;

public interface IStripeCustomerRepository : IRepository<StripeCustomerEntity>
{
    /// <summary>Finds the Stripe customer mapping for an application account.</summary>
    Task<StripeCustomerEntity?> GetByUserIdAsync(Guid userId, CancellationToken token = default);

    /// <summary>Finds the mapping by Stripe customer id (webhook enrichment).</summary>
    Task<StripeCustomerEntity?> GetByStripeCustomerIdAsync(string stripeCustomerId, CancellationToken token = default);
}
