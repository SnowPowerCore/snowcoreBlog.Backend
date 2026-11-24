using snowcoreBlog.Backend.Stripe.Core.Entities.Stripe;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Stripe.Context;

/// <summary>
/// Carrier for the goods-checkout pipeline. The request part is immutable, step
/// outputs (the payment record) are mutable so steps can be re-ordered or extended
/// (e.g. inventory reservation, tax calculation) without changing the delegate.
/// </summary>
public class CreateGoodsCheckoutContext(CreateGoodsCheckoutDto request)
{
    // Request (immutable part)
    public CreateGoodsCheckoutDto Request { get; } = request;

    // Filled by steps вЂ” the entity is born in the session-creation step, once the
    // Checkout Session id/url are known (they are required members).
    public PaymentEntity? Payment { get; set; }
}
