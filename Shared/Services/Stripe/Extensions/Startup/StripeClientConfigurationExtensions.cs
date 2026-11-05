using Microsoft.Extensions.Options;
using Stripe;
using snowcoreBlog.Backend.Stripe.Interfaces.Services;
using snowcoreBlog.Backend.Stripe.Options;
using snowcoreBlog.Backend.Stripe.Services;

namespace snowcoreBlog.Backend.Stripe.Extensions.Startup;

public static class StripeClientConfigurationExtensions
{
    public static IServiceCollection AddStripeClientConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        string section = "Stripe")
    {
        // Stripe's request-latency telemetry phones home to Stripe on every API call;
        // disable it — we run our own OpenTelemetry pipeline.
        StripeConfiguration.EnableTelemetry = false;

        // The client (and the Stripe service classes depending on it) is resolved
        // lazily: the host starts fine without credentials — ping/OpenAPI/webhook
        // (verification guard) keep working; only checkout endpoints demand a key.
        services.AddSingleton<IStripeClient>(static sp =>
        {
            var options = sp.GetRequiredService<IOptions<StripeOptions>>().Value;
            if (string.IsNullOrWhiteSpace(options.SecretKey))
            {
                throw new InvalidOperationException(
                    "Stripe:SecretKey is not configured. Provide it via user secrets, environment or configuration before using checkout endpoints.");
            }

            return new StripeClient(options.SecretKey);
        });

        // Typed Stripe API service facades (all scoped-safe; wrap the shared client).
        services.AddSingleton<global::Stripe.Checkout.SessionService>();
        services.AddSingleton<CustomerService>();
        services.AddSingleton<PaymentIntentService>();
        services.AddSingleton<ChargeService>();

        services.AddSingleton<IStripeCustomerService, StripeCustomerService>();
        services.AddSingleton<IStripeWebhookProcessor, StripeWebhookProcessor>();

        return services;
    }
}
