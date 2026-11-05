using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Stripe.Context;
using snowcoreBlog.Backend.Stripe.Delegates;
using snowcoreBlog.Backend.Stripe.Options;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Stripe.Steps.Donations;

/// <summary>
/// Terminal step of the donation pipeline: maps the persisted entity to the public
/// checkout result handed back to the caller (which redirects to <c>CheckoutUrl</c>).
/// </summary>
public sealed class BuildDonationCheckoutResultStep(IOptions<StripeOptions> options) : IStep<CreateDonationCheckoutDelegate, CreateDonationCheckoutContext, IMaybe<DonationCheckoutResultDto>>
{
    public Task<IMaybe<DonationCheckoutResultDto>> InvokeAsync(CreateDonationCheckoutContext context, CreateDonationCheckoutDelegate next, CancellationToken token = default)
    {
        var donation = context.Donation
            ?? throw new InvalidOperationException("Donation pipeline finished without a donation record.");

        return Task.FromResult(Maybe.Create(new DonationCheckoutResultDto
        {
            DonationId = donation.Id,
            CheckoutSessionId = donation.CheckoutSessionId,
            CheckoutUrl = donation.CheckoutUrl,
            ExpiresAt = donation.CreatedAt.AddMinutes(options.Value.CheckoutSessionExpirationMinutes)
        }));
    }
}
