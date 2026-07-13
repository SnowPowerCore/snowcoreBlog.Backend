using Ixnas.AltchaNet;
using Marten;
using Marten.Services;
using snowcoreBlog.Backend.Core.Entities.Reader;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Entities;
using snowcoreBlog.Backend.Infrastructure.Stores;
using snowcoreBlog.Backend.ReadersManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.ReadersManagement.Repositories.Marten;
using System.Security.Cryptography;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions.Startup;

public static class MartenConfigurationExtensions
{
    private static readonly SystemTextJsonSerializer _serializer = new();

    public static IServiceCollection AddMartenConfiguration(
        this IServiceCollection services)
    {
        services.AddSingleton(static sp =>
        {
            var key = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(key);
            return Altcha.CreateServiceBuilder()
                .UseSha256(key)
                .SetExpiryInSeconds(30)
                .UseStore(() =>
                {
                    using var scope = sp.CreateScope();
                    return scope.ServiceProvider.GetRequiredService<IAltchaCancellableChallengeStore>();
                })
                .Build();
        });

        services.AddMarten(options =>
        {
            options.RegisterDocumentType<ReaderEntity>();
            options.RegisterDocumentType<AltchaStoredChallengeEntity>();
            _serializer.UseTypeInfoResolver(CoreSerializationContext.Default);
            options.Serializer(_serializer);
            options.Policies.AllDocumentsSoftDeleted();
        })
            .UseLightweightSessions()
            .UseNpgsqlDataSource();

        services.AddScoped<IAltchaCancellableChallengeStore, AltchaChallengeStore>();
        services.AddScoped<IReaderRepository, ReaderRepository>();

        return services;
    }
}