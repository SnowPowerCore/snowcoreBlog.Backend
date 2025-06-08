using MassTransit;
using Microsoft.Extensions.Logging;
using Stripe;

namespace StripeMicroservice.Consumers;

public class StripePaymentIntentConsumer : IConsumer<StripePaymentIntent>
{
    private readonly IStripeClient _stripeClient;
    private readonly ILogger<StripePaymentIntentConsumer> _logger;

    public StripePaymentIntentConsumer(IStripeClient stripeClient, ILogger<StripePaymentIntentConsumer> logger)
    {
        _stripeClient = stripeClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<StripePaymentIntent> context)
    {
        var paymentIntentService = new PaymentIntentService(_stripeClient);
        var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = context.Message.Amount,
            Currency = context.Message.Currency,
            Customer = context.Message.CustomerId
        });
        _logger.LogInformation($"Created Stripe payment intent with ID: {paymentIntent.Id}");
        // Optionally respond or publish event
    }
}

public class StripePaymentIntent
{
    public long Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
}
