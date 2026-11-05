using snowcoreBlog.Backend.Core.Entities.Stripe;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Stripe.Context;

/// <summary>
/// Carrier for the donation-checkout pipeline. The request part is immutable,
/// step outputs (the donation record) are mutable so steps can be re-ordered or
/// extended (e.g. fraud screening, recurring-donation upgrade) without changing
/// the delegate signature.
/// </summary>
public class CreateDonationCheckoutContext(CreateDonationCheckoutDto request)
{
    // Request (immutable part)
    public CreateDonationCheckoutDto Request { get; } = request;

    // Filled by steps — the entity is born in the session-creation step, once the
    // Checkout Session id/url are known (they are required members).
    public DonationEntity? Donation { get; set; }
}
