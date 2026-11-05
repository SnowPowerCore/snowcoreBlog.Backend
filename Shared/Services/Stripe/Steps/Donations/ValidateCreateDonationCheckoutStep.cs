using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Stripe.Context;
using snowcoreBlog.Backend.Stripe.Delegates;
using snowcoreBlog.Backend.Stripe.Options;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Stripe.Steps.Donations;

/// <summary>
/// Fail-fast gate of the donation pipeline: amount bounds and currency allowlist
/// are enforced server-side (never trust the client) before any Stripe call.
/// </summary>
public sealed class ValidateCreateDonationCheckoutStep(IOptions<StripeOptions> options) : IStep<CreateDonationCheckoutDelegate, CreateDonationCheckoutContext, IMaybe<DonationCheckoutResultDto>>
{
    public Task<IMaybe<DonationCheckoutResultDto>> InvokeAsync(CreateDonationCheckoutContext context, CreateDonationCheckoutDelegate next, CancellationToken token = default)
    {
        var limits = options.Value;
        var request = context.Request;

        if (request.AmountMinor < limits.DonationMinAmountMinor)
        {
            return Task.FromResult(DonationError<DonationCheckoutResultDto>.Create(
                $"Donation must be at least {limits.DonationMinAmountMinor} minor units."));
        }

        if (request.AmountMinor > limits.DonationMaxAmountMinor)
        {
            return Task.FromResult(DonationError<DonationCheckoutResultDto>.Create(
                $"Donation must not exceed {limits.DonationMaxAmountMinor} minor units."));
        }

        var currency = string.IsNullOrWhiteSpace(request.Currency)
            ? limits.DefaultCurrency
            : request.Currency.ToLowerInvariant();

        if (limits.AllowedCurrencies.Length > 0 &&
            !limits.AllowedCurrencies.Contains(currency, StringComparer.OrdinalIgnoreCase))
        {
            return Task.FromResult(DonationError<DonationCheckoutResultDto>.Create(
                $"Currency '{currency}' is not accepted. Allowed: {string.Join(", ", limits.AllowedCurrencies)}."));
        }

        return next(context, token);
    }
}
