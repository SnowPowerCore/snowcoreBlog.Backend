using Marten;
using snowcoreBlog.Backend.Stripe.Core.Entities.Stripe;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;
using snowcoreBlog.Backend.Stripe.Interfaces.Repositories.Marten;

namespace snowcoreBlog.Backend.Stripe.Repositories.Marten;

public class DonationRepository(IDocumentSession session) : BaseMartenRepository<DonationEntity>(session), IDonationRepository
{
    private readonly IDocumentSession _session = session;

    public Task<DonationEntity?> GetByCheckoutSessionIdAsync(string checkoutSessionId, CancellationToken token = default) =>
        _session.Query<DonationEntity>()
            .FirstOrDefaultAsync(d => d.CheckoutSessionId == checkoutSessionId, token);
}
