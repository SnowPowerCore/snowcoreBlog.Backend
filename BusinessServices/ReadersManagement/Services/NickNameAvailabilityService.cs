using ActualLab.Fusion;
using MaybeResults;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Steps.NickName;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Services;

namespace snowcoreBlog.Backend.ReadersManagement.Services;

/// <summary>
/// Server-side Fusion compute service backing <see cref="INickNameAvailabilityService"/>.
/// </summary>
/// <remarks>
/// <para>
/// <b>Why a compute service?</b> The result of <see cref="GetAsync"/> is memoized by
/// Fusion keyed on the normalized nickname. Every connected client (Blazor Server and
/// WebAssembly) shares the same consistent cached value, and a single
/// <see cref="NotifyReservedAsync"/> invalidation cascades to all observers — so the UI
/// updates in real time without polling or manual refresh.
/// </para>
/// <para>
/// <b>Source of truth.</b> Reuses the existing MassTransit
/// <c>ValidateUserNickNameTaken</c> request to the IAM service, which checks both the
/// ASP.NET Identity user store and the Marten-backed <c>ApplicationTempUser</c> table.
/// This keeps one consistent definition of "taken" across the legacy REST endpoint and
/// the new real-time channel.
/// </para>
/// </remarks>
public class NickNameAvailabilityService(ILogger<NickNameAvailabilityService> logger) : INickNameAvailabilityService
{
    [StepifiedProcess(Steps = [
        typeof(ValidateNickNameWasNotTakenStep),
    ])]
    protected CheckNickNameNotTakenDelegate CheckNickNameNotTaken { get; }

    [ComputeMethod]
    public virtual async Task<NickNameAvailability> GetAsync(
        string nickName, CancellationToken cancellationToken = default)
    {
        var candidate = nickName?.Trim() ?? string.Empty;
        var context = new CheckNickNameNotTakenContext(candidate);

        var result = await CheckNickNameNotTaken(context, cancellationToken);

        var now = DateTimeOffset.UtcNow;
        if (result is Some<NickNameNotTakenCheckResultDto> successResult)
        {
            return successResult.Value.WasTaken
                ? NickNameAvailability.Taken(now)
                : NickNameAvailability.Available(now);
        }
        else if (result is INone<NickNameNotTakenCheckResultDto> noneResult)
        {
            logger.LogWarning(
                "NickName availability check failed for '{candidate}': {Message}",
                candidate, noneResult.Message);
            // On failure we report "taken"-equivalent conservatively is wrong for UX;
            // surface Idle so the UI re-tries on the next keystroke rather than
            // blocking registration on a transient error.
            return NickNameAvailability.Idle with { CheckedAt = now };
        }
        return NickNameAvailability.Idle;
    }

    /// <inheritdoc />
    public Task NotifyReservedAsync(string nickName, CancellationToken cancellationToken = default)
    {
        var candidate = nickName?.Trim() ?? string.Empty;
        if (candidate.Length == 0)
            return Task.CompletedTask;

        // Invalidation scope: re-invoking the compute method here (without awaiting it)
        // inside Invalidating() tells Fusion "the cached value for these exact arguments
        // is no longer consistent", which cascades to every dependent client.
        using (Invalidation.Begin())
        {
            // Touch every [ComputeMethod] result that depends on this nickname.
            // The call does not run; it just registers the invalidation key.
            _ = GetAsync(candidate, cancellationToken);
        }

        return Task.CompletedTask;
    }
}
