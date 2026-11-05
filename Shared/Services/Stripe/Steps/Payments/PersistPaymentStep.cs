using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Stripe.Context;
using snowcoreBlog.Backend.Stripe.Delegates;
using snowcoreBlog.Backend.Stripe.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Stripe.Steps.Payments;

/// <summary>
/// Registers the payment record in Marten BEFORE the checkout URL leaves the
/// service — the webhook racing the HTTP response always finds its record.
/// </summary>
public sealed class PersistPaymentStep(IPaymentRepository paymentRepository) : IStep<CreateGoodsCheckoutDelegate, CreateGoodsCheckoutContext, IMaybe<GoodsCheckoutResultDto>>
{
    public async Task<IMaybe<GoodsCheckoutResultDto>> InvokeAsync(CreateGoodsCheckoutContext context, CreateGoodsCheckoutDelegate next, CancellationToken token = default)
    {
        if (context.Payment is null)
        {
            return PaymentError<GoodsCheckoutResultDto>.Create("Checkout session was not created; nothing to persist.");
        }

        await paymentRepository.AddOrUpdateAsync(context.Payment, context.Payment.Id, true, token);

        return await next(context, token);
    }
}
