namespace snowcoreBlog.Backend.Stripe.Interfaces.Services;

/// <summary>
/// Verifies, deduplicates and applies Stripe webhook deliveries to the local
/// donation/payment records. One call = one atomic transaction (Marten session)
/// covering both the entity transition and the webhook-ledger row.
/// </summary>
public interface IStripeWebhookProcessor
{
    Task<StripeWebhookResult> ProcessAsync(string json, string stripeSignatureHeader, CancellationToken token = default);
}
