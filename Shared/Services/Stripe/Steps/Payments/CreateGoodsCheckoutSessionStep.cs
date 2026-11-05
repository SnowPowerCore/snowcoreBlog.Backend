using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using snowcoreBlog.Backend.Core.Entities.Stripe;
using snowcoreBlog.Backend.Stripe.Context;
using snowcoreBlog.Backend.Stripe.Delegates;
using snowcoreBlog.Backend.Stripe.Interfaces.Services;
using snowcoreBlog.Backend.Stripe.Options;
using snowcoreBlog.Backend.Stripe.Services;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Stripe.Steps.Payments;

/// <summary>
/// Creates the Stripe Checkout Session (payment mode, pay submit type) for goods
/// and the local <see cref="PaymentEntity"/> in one go. When the buyer is a known
/// account (id + e-mail present) the payment is attached to that Stripe customer;
/// otherwise it stays a guest checkout and Checkout collects the e-mail.
/// </summary>
public sealed class CreateGoodsCheckoutSessionStep(
    IOptions<StripeOptions> options,
    global::Stripe.Checkout.SessionService sessionService,
    IStripeCustomerService customerService,
    ILogger<CreateGoodsCheckoutSessionStep> logger) : IStep<CreateGoodsCheckoutDelegate, CreateGoodsCheckoutContext, IMaybe<GoodsCheckoutResultDto>>
{
    public async Task<IMaybe<GoodsCheckoutResultDto>> InvokeAsync(CreateGoodsCheckoutContext context, CreateGoodsCheckoutDelegate next, CancellationToken token = default)
    {
        var settings = options.Value;
        var request = context.Request;
        var paymentId = Guid.NewGuid();
        var currency = string.IsNullOrWhiteSpace(request.Currency)
            ? settings.DefaultCurrency
            : request.Currency.ToLowerInvariant();
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(settings.CheckoutSessionExpirationMinutes);

        // Attach to a Stripe customer only when BOTH identity hints are present —
        // otherwise this is a guest checkout and Checkout collects the e-mail itself.
        string? stripeCustomerId = null;
        if (request.BuyerUserId is not null && !string.IsNullOrWhiteSpace(request.BuyerEmail))
        {
            try
            {
                var customer = await customerService.EnsureAsync(request.BuyerUserId, request.BuyerEmail, name: null, token);
                stripeCustomerId = customer.StripeCustomerId;
            }
            catch (StripeException ex)
            {
                // Customer creation must never block the checkout — fall back to guest.
                logger.LogWarning(ex, "Could not ensure Stripe customer for buyer {BuyerUserId}; falling back to guest checkout.", request.BuyerUserId);
            }
        }

        var metadata = new Dictionary<string, string>
        {
            [StripeWebhookProcessor.MetadataRecordIdKey] = paymentId.ToString(),
            [StripeWebhookProcessor.MetadataRecordKindKey] = StripeWebhookProcessor.KindPayment,
            ["reference_id"] = request.ReferenceId
        };

        var sessionOptions = new SessionCreateOptions
        {
            Mode = "payment",
            SubmitType = "pay",
            ClientReferenceId = paymentId.ToString(),
            Customer = stripeCustomerId,
            CustomerEmail = stripeCustomerId is null ? request.BuyerEmail : null,
            SuccessUrl = request.SuccessUrl ?? settings.SuccessUrl,
            CancelUrl = request.CancelUrl ?? settings.CancelUrl,
            ExpiresAt = expiresAt,
            Metadata = metadata,
            PaymentIntentData = new SessionPaymentIntentDataOptions
            {
                Description = request.Description,
                ReceiptEmail = request.BuyerEmail,
                Metadata = new Dictionary<string, string>(metadata)
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
                            Name = settings.GoodsProductName,
                            Description = request.Description
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
            logger.LogError(ex, "Stripe rejected goods checkout creation: {Message}", ex.Message);
            return PaymentError<GoodsCheckoutResultDto>.Create(
                $"Stripe rejected the checkout request: {ex.StripeError?.Message ?? ex.Message}");
        }

        context.Payment = new PaymentEntity
        {
            Id = paymentId,
            ReferenceId = request.ReferenceId,
            Description = request.Description,
            CheckoutSessionId = session.Id,
            CheckoutUrl = session.Url,
            StripeCustomerId = stripeCustomerId,
            BuyerUserId = request.BuyerUserId,
            BuyerEmail = request.BuyerEmail,
            AmountMinor = request.AmountMinor,
            Currency = currency,
            Status = StripePaymentStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow
        };

        return await next(context, token);
    }
}
