using Marten;
using snowcoreBlog.Backend.Stripe.Core.Entities.Stripe;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;
using snowcoreBlog.Backend.Stripe.Interfaces.Repositories.Marten;

namespace snowcoreBlog.Backend.Stripe.Repositories.Marten;

public class StripeCustomerRepository(IDocumentSession session) : BaseMartenRepository<StripeCustomerEntity>(session), IStripeCustomerRepository
{
    private readonly IDocumentSession _session = session;

    public async Task<StripeCustomerEntity?> GetByUserIdAsync(Guid userId, CancellationToken token = default) =>
        await _session.Query<StripeCustomerEntity>()
            .FirstOrDefaultAsync(c => c.UserId == userId, token);

    public async Task<StripeCustomerEntity?> GetByStripeCustomerIdAsync(string stripeCustomerId, CancellationToken token = default) =>
        await _session.Query<StripeCustomerEntity>()
            .FirstOrDefaultAsync(c => c.StripeCustomerId == stripeCustomerId, token);
}
