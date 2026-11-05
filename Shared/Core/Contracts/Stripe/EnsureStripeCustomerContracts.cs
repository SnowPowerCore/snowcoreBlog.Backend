namespace snowcoreBlog.Backend.Core.Contracts.Stripe;

/// <summary>
/// Command (request/response via MassTransit) asking the Stripe microservice to make
/// sure a Stripe Customer exists for the given application account, creating it on
/// demand. Consumers reply with <see cref="StripeCustomerEnsured"/>.
/// </summary>
public sealed record EnsureStripeCustomer(
    Guid UserId,
    string Email,
    string Name);

/// <summary>
/// Reply produced by the Stripe microservice once a Stripe Customer is guaranteed to
/// exist for the requested application account.
/// </summary>
public sealed record StripeCustomerEnsured(
    Guid UserId,
    string Email,
    string StripeCustomerId);
