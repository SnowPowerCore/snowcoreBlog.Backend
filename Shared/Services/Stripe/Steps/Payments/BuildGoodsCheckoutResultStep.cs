using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Stripe.Context;
using snowcoreBlog.Backend.Stripe.Delegates;
using snowcoreBlog.Backend.Stripe.Options;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Stripe.Steps.Payments;

/// <summary>
/// Terminal step of the goods pipeline: maps the persisted entity to the public
/// checkout result handed back to the caller (which redirects to <c>CheckoutUrl</c>).
/// </summary>
public sealed class BuildGoodsCheckoutResultStep(IOptions<StripeOptions> options) : IStep<CreateGoodsCheckoutDelegate, CreateGoodsCheckoutContext, IMaybe<GoodsCheckoutResultDto>>
{
    public Task<IMaybe<GoodsCheckoutResultDto>> InvokeAsync(CreateGoodsCheckoutContext context, CreateGoodsCheckoutDelegate next, CancellationToken token = default)
    {
        var payment = context.Payment
            ?? throw new InvalidOperationException("Goods pipeline finished without a payment record.");

        return Task.FromResult(Maybe.Create(new GoodsCheckoutResultDto
        {
            PaymentId = payment.Id,
            ReferenceId = payment.ReferenceId,
            CheckoutSessionId = payment.CheckoutSessionId,
            CheckoutUrl = payment.CheckoutUrl,
            ExpiresAt = payment.CreatedAt.AddMinutes(options.Value.CheckoutSessionExpirationMinutes)
        }));
    }
}
