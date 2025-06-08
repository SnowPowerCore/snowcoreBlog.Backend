using MassTransit;
using Microsoft.Extensions.Logging;
using Stripe;

namespace StripeMicroservice.Consumers;

public class StripeCustomerCreatedConsumer : IConsumer<StripeCustomerCreated>
{
    private readonly IStripeClient _stripeClient;
    private readonly ILogger<StripeCustomerCreatedConsumer> _logger;

    public StripeCustomerCreatedConsumer(IStripeClient stripeClient, ILogger<StripeCustomerCreatedConsumer> logger)
    {
        _stripeClient = stripeClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<StripeCustomerCreated> context)
    {
        var customerService = new CustomerService(_stripeClient);
        var customer = await customerService.CreateAsync(new CustomerCreateOptions
        {
            Email = context.Message.Email,
            Name = context.Message.Name
        });
        _logger.LogInformation($"Created Stripe customer with ID: {customer.Id}");
        // Optionally respond or publish event
    }
}

public class StripeCustomerCreated
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
