using snowcoreBlog.Backend.Core.Entities.Stripe;
using snowcoreBlog.Backend.Core.Interfaces.Repositories;

namespace snowcoreBlog.Backend.Stripe.Interfaces.Repositories.Marten;

public interface IDonationRepository : IRepository<DonationEntity>
{
    /// <summary>Finds a donation by its Stripe Checkout Session id (webhook reconciliation).</summary>
    Task<DonationEntity?> GetByCheckoutSessionIdAsync(string checkoutSessionId, CancellationToken token = default);
}
