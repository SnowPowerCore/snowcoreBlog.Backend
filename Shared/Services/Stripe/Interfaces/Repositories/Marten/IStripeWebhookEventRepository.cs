using snowcoreBlog.Backend.Stripe.Core.Entities.Stripe;
using snowcoreBlog.Backend.Shared.Core.Interfaces.Repositories;

namespace snowcoreBlog.Backend.Stripe.Interfaces.Repositories.Marten;

public interface IStripeWebhookEventRepository : IRepository<StripeWebhookEventEntity>
{
    /// <summary>Returns true when the Stripe event id was already processed (replay).</summary>
    Task<bool> ExistsByEventIdAsync(string eventId, CancellationToken token = default);
}
