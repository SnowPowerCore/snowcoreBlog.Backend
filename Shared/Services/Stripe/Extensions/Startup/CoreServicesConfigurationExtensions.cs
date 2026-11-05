using snowcoreBlog.Backend.Stripe.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Stripe.Repositories.Marten;
using snowcoreBlog.Backend.Stripe.Steps.Donations;
using snowcoreBlog.Backend.Stripe.Steps.Payments;

namespace snowcoreBlog.Backend.Stripe.Extensions.Startup;

public static class CoreServicesConfigurationExtensions
{
    public static IServiceCollection AddCoreServicesConfiguration(this IServiceCollection services)
    {
        // Marten repositories
        services.AddScoped<IDonationRepository, DonationRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IStripeCustomerRepository, StripeCustomerRepository>();
        services.AddScoped<IStripeWebhookEventRepository, StripeWebhookEventRepository>();

        // Donation checkout pipeline steps (order matters; keep in sync with CreateDonationCheckoutEndpoint)
        services.AddScoped<ValidateCreateDonationCheckoutStep>();
        services.AddScoped<CreateDonationCheckoutSessionStep>();
        services.AddScoped<PersistDonationStep>();
        services.AddScoped<BuildDonationCheckoutResultStep>();

        // Goods checkout pipeline steps (order matters; keep in sync with CreateGoodsCheckoutEndpoint)
        services.AddScoped<ValidateCreateGoodsCheckoutStep>();
        services.AddScoped<CreateGoodsCheckoutSessionStep>();
        services.AddScoped<PersistPaymentStep>();
        services.AddScoped<BuildGoodsCheckoutResultStep>();

        return services;
    }
}
