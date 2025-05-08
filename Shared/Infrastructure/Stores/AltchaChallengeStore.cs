using Ixnas.AltchaNet;
using Marten;
using Npgsql;
using snowcoreBlog.Backend.Infrastructure.Entities;

namespace snowcoreBlog.Backend.Infrastructure.Stores;

public class AltchaChallengeStore : IAltchaCancellableChallengeStore
{
    private readonly NpgsqlDataSource _dataSource;

    public AltchaChallengeStore(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<bool> Exists(string challenge, CancellationToken cancellationToken)
    {
        using var store = DocumentStore.For(opts =>
        {
            opts.Connection(_dataSource);
        });
        await using var session = store.LightweightSession();
        session.DeleteWhere<AltchaStoredChallengeEntity>(storedChallenge =>
            storedChallenge.ExpiryUtc <= DateTimeOffset.UtcNow);
        await session.SaveChangesAsync(cancellationToken);
        return session.Query<AltchaStoredChallengeEntity>().Any(storedChallenge =>
            storedChallenge.Challenge == challenge);
    }

    public async Task Store(string challenge, DateTimeOffset expiryUtc, CancellationToken cancellationToken)
    {
        using var store = DocumentStore.For(opts =>
        {
            opts.Connection(_dataSource);
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