namespace snowcoreBlog.Backend.Core.Contracts.Stripe;

/// <summary>
/// Reply produced by the Stripe microservice once a Stripe Customer is guaranteed to
/// exist for the requested application account.
/// </summary>
public sealed record StripeCustomerEnsured(
    Guid UserId,
    string Email,
    string StripeCustomerId);
