using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using snowcoreBlog.Backend.Stripe.Core.Entities.Stripe;
using snowcoreBlog.Backend.Stripe.Context;
using snowcoreBlog.Backend.Stripe.Delegates;
using snowcoreBlog.Backend.Stripe.Options;
using snowcoreBlog.Backend.Stripe.Services;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Stripe.Steps.Donations;

/// <summary>
/// Creates the Stripe Checkout Session (payment mode, donate submit type) and the
/// local <see cref="DonationEntity"/> in one go. The record id travels inside the
/// session (<c>client_reference_id</c> + metadata) so every webhook payload can be
/// traced back here, and the entity is persisted by the NEXT step вЂ” the record
/// exists before the checkout URL is handed out, so a webhook racing the HTTP
/// response always finds it.
/// </summary>
public sealed class CreateDonationCheckoutSessionStep(
    IOptions<StripeOptions> options,
    global::Stripe.Checkout.SessionService sessionService,
    ILogger<CreateDonationCheckoutSessionStep> logger) : IStep<CreateDonationCheckoutDelegate, CreateDonationCheckoutContext, IMaybe<DonationCheckoutResultDto>>
{
    public async Task<IMaybe<DonationCheckoutResultDto>> InvokeAsync(CreateDonationCheckoutContext context, CreateDonationCheckoutDelegate next, CancellationToken token = default)
    {
        var settings = options.Value;
        var request = context.Request;
        var donationId = Guid.NewGuid();
        var currency = string.IsNullOrWhiteSpace(request.Currency)
            ? settings.DefaultCurrency
            : request.Currency.ToLowerInvariant();
        var expiresAt = DateTime.UtcNow.AddMinutes(settings.CheckoutSessionExpirationMinutes);

        var sessionOptions = new SessionCreateOptions
        {
            Mode = "payment",
            SubmitType = "donate",
            ClientReferenceId = donationId.ToString(),
            CustomerEmail = request.DonorEmail,
            SuccessUrl = request.SuccessUrl ?? settings.SuccessUrl,
            CancelUrl = request.CancelUrl ?? settings.CancelUrl,
            ExpiresAt = expiresAt,
            Metadata = new Dictionary<string, string>
            {
                [StripeWebhookProcessor.MetadataRecordIdKey] = donationId.ToString(),
                [StripeWebhookProcessor.MetadataRecordKindKey] = StripeWebhookProcessor.KindDonation
            },
            PaymentIntentData = new SessionPaymentIntentDataOptions
            {
                Description = $"{settings.DonationProductName} (donation {donationId})",
                ReceiptEmail = request.DonorEmail,
                Metadata = new Dictionary<string, string>
                {
                    [StripeWebhookProcessor.MetadataRecordIdKey] = donationId.ToString(),
                    [StripeWebhookProcessor.MetadataRecordKindKey] = StripeWebhookProcessor.KindDonation
                }
            },
            LineItems =
            [
                new SessionLineItemOptions
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = currency,
                        UnitAmount = request.AmountMinor,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = settings.DonationProductName,
                            Description = string.IsNullOrWhiteSpace(request.Message)
                                ? null
                                : $"Donor note: {request.Message}"
                        }
                    }
                }
            ]
        };

        global::Stripe.Checkout.Session session;
        try
        {
            session = await sessionService.CreateAsync(sessionOptions, cancellationToken: token);
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Stripe rejected donation checkout creation: {Message}", ex.Message);
            return DonationError<DonationCheckoutResultDto>.Create(
                $"Stripe rejected the checkout request: {ex.StripeError?.Message ?? ex.Message}");
        }

        context.Donation = new DonationEntity
        {
            Id = donationId,
            CheckoutSessionId = session.Id,
            CheckoutUrl = session.Url,
            DonorUserId = request.DonorUserId,
            DonorEmail = request.DonorEmail,
            AmountMinor = request.AmountMinor,
            Currency = currency,
            Status = StripePaymentStatus.Pending,
            Message = string.IsNullOrWhiteSpace(request.Message) ? null : request.Message,
            Metadata = request.DonorUserId is null ? null : new Dictionary<string, string> { ["donor_user_id"] = request.DonorUserId.Value.ToString() },
            CreatedAt = DateTime.UtcNow
        };

        return await next(context, token);
    }
}
