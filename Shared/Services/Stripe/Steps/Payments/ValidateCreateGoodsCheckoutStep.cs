using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Stripe.Context;
using snowcoreBlog.Backend.Stripe.Delegates;
using snowcoreBlog.Backend.Stripe.Options;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Stripe.Steps.Payments;

/// <summary>
/// Fail-fast gate of the goods pipeline: amount bounds, currency allowlist and
/// reference sanity are enforced server-side before any Stripe call.
/// </summary>
public sealed class ValidateCreateGoodsCheckoutStep(IOptions<StripeOptions> options) : IStep<CreateGoodsCheckoutDelegate, CreateGoodsCheckoutContext, IMaybe<GoodsCheckoutResultDto>>
{
    public Task<IMaybe<GoodsCheckoutResultDto>> InvokeAsync(CreateGoodsCheckoutContext context, CreateGoodsCheckoutDelegate next, CancellationToken token = default)
    {
        var limits = options.Value;
        var request = context.Request;

        if (string.IsNullOrWhiteSpace(request.ReferenceId))
        {
            return Task.FromResult(PaymentError<GoodsCheckoutResultDto>.Create("ReferenceId is required to correlate the payment with the order."));
        }

        if (request.AmountMinor < limits.GoodsMinAmountMinor)
        {
            return Task.FromResult(PaymentError<GoodsCheckoutResultDto>.Create(
                $"Payment must be at least {limits.GoodsMinAmountMinor} minor units."));
        }

        if (request.AmountMinor > limits.GoodsMaxAmountMinor)
        {
            return Task.FromResult(PaymentError<GoodsCheckoutResultDto>.Create(
                $"Payment must not exceed {limits.GoodsMaxAmountMinor} minor units."));
        }

        var currency = string.IsNullOrWhiteSpace(request.Currency)
            ? limits.DefaultCurrency
            : request.Currency.ToLowerInvariant();

        if (limits.AllowedCurrencies.Length > 0 &&
            !limits.AllowedCurrencies.Contains(currency, StringComparer.OrdinalIgnoreCase))
        {
            return Task.FromResult(PaymentError<GoodsCheckoutResultDto>.Create(
                $"Currency '{currency}' is not accepted. Allowed: {string.Join(", ", limits.AllowedCurrencies)}."));
        }

        return next(context, token);
    }
}
