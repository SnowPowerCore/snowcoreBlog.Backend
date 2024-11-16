using Ixnas.AltchaNet;
using Marten;
using snowcoreBlog.Backend.Infrastructure.Models;

namespace snowcoreBlog.Backend.Infrastructure.Stores;

public class AltchaChallengeStore(IDocumentSession documentSession) : IAltchaChallengeStore
{
    private readonly IDocumentSession _documentSession = documentSession;

    public Task<bool> Exists(string challenge)
    {
        _documentSession.DeleteWhere<AltchaStoredChallenge>(storedChallenge =>
            storedChallenge.ExpiryUtc <= DateTimeOffset.UtcNow);
        var exists = _documentSession.Query<AltchaStoredChallenge>().Any(storedChallenge =>
            storedChallenge.Challenge == challenge);
        return Task.FromResult(exists);
    }

    public async Task Store(string challenge, DateTimeOffset expiryUtc)
    {
        using var ct = new CancellationTokenSource();
        _documentSession.Store(new AltchaStoredChallenge()
        {
            Challenge = challenge,
            ExpiryUtc = expiryUtc
        });
        await _documentSession.SaveChangesAsync(ct.Token);
    }
}