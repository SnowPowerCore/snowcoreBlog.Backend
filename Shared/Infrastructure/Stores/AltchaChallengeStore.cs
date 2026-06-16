using Ixnas.AltchaNet;
using Marten;
using Marten.Services;
using Microsoft.Extensions.Configuration;
using snowcoreBlog.Backend.Infrastructure.Entities;

namespace snowcoreBlog.Backend.Infrastructure.Stores;

public class AltchaChallengeStore : IAltchaCancellableChallengeStore
{
    private readonly SystemTextJsonSerializer _serializer = new();

    private readonly string _connectionString = string.Empty;

    public AltchaChallengeStore(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("db-snowcore-blog-entities")!;
        _serializer.UseTypeInfoResolver(CoreSerializationContext.Default);
    }

    public async Task<bool> Exists(string challenge, CancellationToken cancellationToken)
    {
        using var store = DocumentStore.For(opts =>
        {
            opts.Connection(_connectionString);
            opts.Serializer(_serializer);
        });
        await using var session = store.LightweightSession();
        session.DeleteWhere<AltchaStoredChallengeEntity>(storedChallenge =>
            storedChallenge.ExpiryUtc <= DateTimeOffset.UtcNow);
        await session.SaveChangesAsync(cancellationToken);
        return await session.Query<AltchaStoredChallengeEntity>().AnyAsync(storedChallenge =>
            storedChallenge.Challenge == challenge, cancellationToken);
    }

    public async Task Store(string challenge, DateTimeOffset expiryUtc, CancellationToken cancellationToken)
    {
        using var store = DocumentStore.For(opts =>
        {
            opts.Connection(_connectionString);
            opts.Serializer(_serializer);
        });
        await using var session = store.LightweightSession();
        session.Store(new AltchaStoredChallengeEntity()
        {
            Challenge = challenge,
            ExpiryUtc = expiryUtc
        });
        await session.SaveChangesAsync(cancellationToken);
    }
}