using Marten;
using snowcoreBlog.Backend.Core.Entities.Stripe;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;
using snowcoreBlog.Backend.Stripe.Interfaces.Repositories.Marten;

namespace snowcoreBlog.Backend.Stripe.Repositories.Marten;

public class PaymentRepository(IDocumentSession session) : BaseMartenRepository<PaymentEntity>(session), IPaymentRepository
{
    private readonly IDocumentSession _session = session;

    public Task<PaymentEntity?> GetByCheckoutSessionIdAsync(string checkoutSessionId, CancellationToken token = default) =>
        _session.Query<PaymentEntity>()
            .FirstOrDefaultAsync(p => p.CheckoutSessionId == checkoutSessionId, token);

    public async Task<IEnumerable<PaymentEntity>> GetByReferenceIdAsync(string referenceId, CancellationToken token = default) =>
        await _session.Query<PaymentEntity>()
            .Where(p => p.ReferenceId == referenceId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(token);
}
