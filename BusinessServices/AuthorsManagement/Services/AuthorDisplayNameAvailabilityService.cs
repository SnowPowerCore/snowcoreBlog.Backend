using ActualLab.Fusion;
using snowcoreBlog.Backend.AuthorsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Services;

namespace snowcoreBlog.Backend.AuthorsManagement.Services;

/// <summary>
/// Server-side Fusion compute service backing <see cref="IAuthorDisplayNameAvailabilityService"/>.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why a compute service?</b> The result of <see cref="GetAsync"/> is memoized by
/// Fusion keyed on the normalized display name, shared across all connected clients
/// (Blazor Server and WebAssembly). A single <see cref="NotifyReservedAsync"/>
/// invalidation cascades to every observer — so the "become an author" page updates
/// in real time without polling or manual refresh.
/// </para>
/// <para>
/// <b>Source of truth.</b> Queries the Marten-backed author directory
/// (<c>IAuthorRepository</c>) directly — the same store the
/// <c>CreateAuthorEntityForExistingUserStep</c> writes to when claiming a name.
/// </para>
/// <para>
/// <b>Lifetime note.</b> Fusion compute services are resolved as singletons, while
/// <c>IAuthorRepository</c> (and its <c>IDocumentSession</c>) is scoped. To avoid a
/// captive scoped dependency the repository is resolved from a fresh scope per
/// check — unlike the nickname availability service, this compute service is
/// intentionally not stepified, because a single repository query needs no step
/// pipeline and per-call scoping is the priority.
/// </para>
/// </remarks>
public class AuthorDisplayNameAvailabilityService(
    ILogger<AuthorDisplayNameAvailabilityService> logger,
    IServiceScopeFactory scopeFactory) : IAuthorDisplayNameAvailabilityService
{
    [ComputeMethod]
    public virtual async Task<AuthorDisplayNameAvailability> GetAsync(
        string displayName, CancellationToken cancellationToken = default)
    {
        var candidate = displayName?.Trim() ?? string.Empty;
        if (candidate.Length == 0)
            return AuthorDisplayNameAvailability.Idle;

        bool wasTaken;
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var authorRepository = scope.ServiceProvider.GetRequiredService<IAuthorRepository>();
            wasTaken = await authorRepository.AnyByDisplayNameAsync(candidate, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogWarning(e,
                "Author display name availability check failed for '{candidate}'", candidate);
            // Surface Idle on transient failures so the UI re-tries on the next
            // keystroke rather than blocking the flow on a backend hiccup.
            return AuthorDisplayNameAvailability.Idle with { CheckedAt = DateTimeOffset.UtcNow };
        }

        var now = DateTimeOffset.UtcNow;
        return wasTaken
            ? AuthorDisplayNameAvailability.Taken(now)
            : AuthorDisplayNameAvailability.Available(now);
    }

    /// <inheritdoc />
    public Task NotifyReservedAsync(string displayName, CancellationToken cancellationToken = default)
    {
        var candidate = displayName?.Trim() ?? string.Empty;
        if (candidate.Length == 0)
            return Task.CompletedTask;

        // Invalidation scope: re-invoking the compute method here (without awaiting it)
        // inside Invalidating() tells Fusion "the cached value for these exact arguments
        // is no longer consistent", which cascades to every dependent client.
        using (Invalidation.Begin())
        {
            // Touch every [ComputeMethod] result that depends on this display name.
            // The call does not run; it just registers the invalidation key.
            _ = GetAsync(candidate, cancellationToken);
        }

        return Task.CompletedTask;
    }
}
