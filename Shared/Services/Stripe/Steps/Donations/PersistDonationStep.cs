using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Stripe.Context;
using snowcoreBlog.Backend.Stripe.Delegates;
using snowcoreBlog.Backend.Stripe.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Stripe.Steps.Donations;

/// <summary>
/// Registers the donation record in Marten BEFORE the checkout URL leaves the
/// service — the webhook racing the HTTP response always finds its record.
/// </summary>
public sealed class PersistDonationStep(IDonationRepository donationRepository) : IStep<CreateDonationCheckoutDelegate, CreateDonationCheckoutContext, IMaybe<DonationCheckoutResultDto>>
{
    public async Task<IMaybe<DonationCheckoutResultDto>> InvokeAsync(CreateDonationCheckoutContext context, CreateDonationCheckoutDelegate next, CancellationToken token = default)
    {
        if (context.Donation is null)
        {
            return DonationError<DonationCheckoutResultDto>.Create("Checkout session was not created; nothing to persist.");
        }

        await donationRepository.AddOrUpdateAsync(context.Donation, context.Donation.Id, true, token);

        return await next(context, token);
    }
}
