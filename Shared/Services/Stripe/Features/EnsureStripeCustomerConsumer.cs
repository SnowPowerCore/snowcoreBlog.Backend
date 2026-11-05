using MassTransit;
using snowcoreBlog.Backend.Core.Contracts.Stripe;
using snowcoreBlog.Backend.Stripe.Interfaces.Services;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.Stripe.Features;

/// <summary>
/// Cross-service request/response entry point: any backend service can ask this
/// microservice to guarantee a Stripe Customer exists for one of its accounts
/// (MassTransit request client over the <c>EnsureStripeCustomer</c> contract).
/// </summary>
public class EnsureStripeCustomerConsumer(IStripeCustomerService customerService) : IConsumer<EnsureStripeCustomer>
{
    public async Task Consume(ConsumeContext<EnsureStripeCustomer> context)
    {
        var customer = await customerService.EnsureAsync(
            context.Message.UserId,
            context.Message.Email,
            context.Message.Name,
            context.CancellationToken);

        await context.RespondAsync(
            new DataResult<StripeCustomerEnsured>(
                new StripeCustomerEnsured(
                    customer.UserId ?? context.Message.UserId,
                    customer.Email,
                    customer.StripeCustomerId)));
    }
}
