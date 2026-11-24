using Marten;
using snowcoreBlog.Backend.Stripe.Core.Entities.Stripe;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;
using snowcoreBlog.Backend.Stripe.Interfaces.Repositories.Marten;

namespace snowcoreBlog.Backend.Stripe.Repositories.Marten;

public class StripeWebhookEventRepository(IDocumentSession session) : BaseMartenRepository<StripeWebhookEventEntity>(session), IStripeWebhookEventRepository
{
    private readonly IDocumentSession _session = session;

    public async Task<bool> ExistsByEventIdAsync(string eventId, CancellationToken token = default) =>
        await _session.Query<StripeWebhookEventEntity>()
            .AnyAsync(e => e.EventId == eventId, token);
}
