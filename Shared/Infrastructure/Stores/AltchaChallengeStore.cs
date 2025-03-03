using Ixnas.AltchaNet;
using Marten;
using Microsoft.Extensions.Configuration;
using snowcoreBlog.Backend.Infrastructure.Entities;

namespace snowcoreBlog.Backend.Infrastructure.Stores;

public class AltchaChallengeStore : IAltchaChallengeStore
{
    private readonly string _connectionString;

    public AltchaChallengeStore(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("db-snowcore-blog-entities")!;
    }

    public async Task<bool> Exists(string challenge)
    {
        using var store = DocumentStore.For(opts =>
        {
            opts.Connection(_connectionString);
        });
        await using var session = store.LightweightSession();
        session.DeleteWhere<AltchaStoredChallengeEntity>(storedChallenge =>
            storedChallenge.ExpiryUtc <= DateTimeOffset.UtcNow);
        return session.Query<AltchaStoredChallengeEntity>().Any(storedChallenge =>
            storedChallenge.Challenge == challenge);
    }

    public async Task Store(string challenge, DateTimeOffset expiryUtc)
    {
        using var store = DocumentStore.For(opts =>
        {
            opts.Connection(_connectionString);
        });
        await using var session = store.LightweightSession();
        using var ct = new CancellationTokenSource();
        session.Store(new AltchaStoredChallengeEntity()
        {
            Challenge = challenge,
            ExpiryUtc = expiryUtc
        });
        await session.SaveChangesAsync(ct.Token);
    }
}