using Stripe;
using snowcoreBlog.Backend.Core.Entities.Stripe;
using snowcoreBlog.Backend.Stripe.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Stripe.Interfaces.Services;

namespace snowcoreBlog.Backend.Stripe.Services;

/// <inheritdoc />
public class StripeCustomerService(
    CustomerService customerService,
    IStripeCustomerRepository customerRepository,
    ILogger<StripeCustomerService> logger) : IStripeCustomerService
{
    public async Task<StripeCustomerEntity> EnsureAsync(Guid? userId, string email, string? name, CancellationToken token = default)
    {
        // 1. Application-account mapping wins — one Stripe customer per account.
        if (userId is not null)
        {
            var existing = await customerRepository.GetByUserIdAsync(userId.Value, token);
            if (existing is not null)
            {
                return existing;
            }
        }

        // 2. Create the Stripe customer (idempotent enough: duplicate e-mail is allowed
        //    on Stripe's side; the local mapping is keyed by account id, not e-mail).
        var customer = await customerService.CreateAsync(new CustomerCreateOptions
        {
            Email = email,
            Name = name,
            Metadata = new Dictionary<string, string>
            {
                ["user_id"] = userId?.ToString() ?? "guest"
            }
        }, cancellationToken: token);

        logger.LogInformation("Created Stripe customer {StripeCustomerId} for account {UserId}", customer.Id, userId);

        var mapping = new StripeCustomerEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Email = email,
            Name = name,
            StripeCustomerId = customer.Id,
            CreatedAt = DateTime.UtcNow
        };

        await customerRepository.AddOrUpdateAsync(mapping, mapping.Id, true, token);

        return mapping;
    }

    public Task<StripeCustomerEntity?> GetByUserIdAsync(Guid userId, CancellationToken token = default) =>
        customerRepository.GetByUserIdAsync(userId, token);
}
