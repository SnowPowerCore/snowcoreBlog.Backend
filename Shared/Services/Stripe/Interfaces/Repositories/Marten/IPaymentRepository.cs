using snowcoreBlog.Backend.Stripe.Core.Entities.Stripe;
using snowcoreBlog.Backend.Shared.Core.Interfaces.Repositories;

namespace snowcoreBlog.Backend.Stripe.Interfaces.Repositories.Marten;

public interface IPaymentRepository : IRepository<PaymentEntity>
{
    /// <summary>Finds a payment by its Stripe Checkout Session id (webhook reconciliation).</summary>
    Task<PaymentEntity?> GetByCheckoutSessionIdAsync(string checkoutSessionId, CancellationToken token = default);

    /// <summary>Finds payments by the caller-supplied external reference (order lookup).</summary>
    Task<IEnumerable<PaymentEntity>> GetByReferenceIdAsync(string referenceId, CancellationToken token = default);
}
