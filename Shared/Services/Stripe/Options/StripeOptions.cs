namespace snowcoreBlog.Backend.Stripe.Options;

/// <summary>
/// Stripe integration options bound from the <c>Stripe</c> configuration section.
/// Secret values (SecretKey, WebhookSecret) should come from user secrets /
/// environment in Development and from the secret store in production — never
/// from source control.
/// </summary>
public sealed record StripeOptions
{
    /// <summary>Stripe secret API key (sk_...). Required — the client cannot start without it.</summary>
    public string SecretKey { get; init; } = string.Empty;

    /// <summary>Signing secret of the webhook endpoint (whsec_...). Required for webhook verification.</summary>
    public string WebhookSecret { get; init; } = string.Empty;

    /// <summary>Default ISO 4217 currency used when a request omits one.</summary>
    public string DefaultCurrency { get; init; } = "usd";

    /// <summary>Lowercase ISO 4217 codes the service is allowed to charge in. Empty = allow all.</summary>
    public string[] AllowedCurrencies { get; init; } = ["usd", "eur"];

    /// <summary>Smallest accepted donation in minor units (100 = 1.00).</summary>
    public long DonationMinAmountMinor { get; init; } = 100;

    /// <summary>Largest accepted donation in minor units.</summary>
    public long DonationMaxAmountMinor { get; init; } = 1_000_000_000;

    /// <summary>Smallest accepted goods payment in minor units (Stripe card minimum is 50).</summary>
    public long GoodsMinAmountMinor { get; init; } = 50;

    /// <summary>Largest accepted goods payment in minor units.</summary>
    public long GoodsMaxAmountMinor { get; init; } = 1_000_000_000;

    /// <summary>Checkout Session lifetime in minutes (Stripe allows 30 minutes to 24 hours).</summary>
    public int CheckoutSessionExpirationMinutes { get; init; } = 60;

    /// <summary>
    /// Default post-payment redirect. The literal <c>{CHECKOUT_SESSION_ID}</c> is
    /// replaced by Stripe; overriding per-request is supported via the DTOs.
    /// </summary>
    public string SuccessUrl { get; init; } = "https://localhost/app/payments/success?session_id={CHECKOUT_SESSION_ID}";

    /// <summary>Default redirect when the payer cancels Checkout.</summary>
    public string CancelUrl { get; init; } = "https://localhost/app/payments/canceled";

    /// <summary>Product name shown on the donation Checkout page and receipts.</summary>
    public string DonationProductName { get; init; } = "Blog donation";

    /// <summary>Product name prefix shown for goods payments without a description.</summary>
    public string GoodsProductName { get; init; } = "Blog purchase";
}
