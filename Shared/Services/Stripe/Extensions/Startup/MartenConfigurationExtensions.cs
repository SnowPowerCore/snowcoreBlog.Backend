using Marten;
using Marten.Services;
using snowcoreBlog.Backend.Stripe.Core.Entities.Stripe;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Stripe.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Stripe.Repositories.Marten;

namespace snowcoreBlog.Backend.Stripe.Extensions.Startup;

public static class MartenConfigurationExtensions
{
    private static readonly SystemTextJsonSerializer _serializer = new();

    public static IServiceCollection AddMartenConfiguration(this IServiceCollection services)
    {
        services.AddMarten(options =>
        {
            options.RegisterDocumentType<StripeCustomerEntity>();
            options.RegisterDocumentType<DonationEntity>();
            options.RegisterDocumentType<PaymentEntity>();
            options.RegisterDocumentType<StripeWebhookEventEntity>();

            // Uniqueness contracts:
            // - one Checkout Session per donation/payment (webhook reconciliation key)
            // - one webhook ledger row per Stripe event id (exactly-once processing)
            // - one Stripe customer mapping per Stripe customer id
            options.Schema.For<DonationEntity>()
                .Index(x => x.CheckoutSessionId, idx => idx.IsUnique = true);
            options.Schema.For<PaymentEntity>()
                .Index(x => x.CheckoutSessionId, idx => idx.IsUnique = true);
            options.Schema.For<StripeWebhookEventEntity>()
                .Index(x => x.EventId, idx => idx.IsUnique = true);
            options.Schema.For<StripeCustomerEntity>()
                .Index(x => x.StripeCustomerId, idx => idx.IsUnique = true);

            _serializer.UseTypeInfoResolver(StripeSerializationContext.Default);
            options.Serializer(_serializer);
            options.Policies.AllDocumentsSoftDeleted();
        })
            .UseLightweightSessions()
            .UseNpgsqlDataSource();

        return services;
    }
}
