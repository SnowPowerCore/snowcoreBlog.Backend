using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Core.Entities.RequestPersistence;
using snowcoreBlog.Backend.Core.Interfaces.Repositories;
using TickerQ.Utilities.Base;
using TickerQ.Utilities.Interfaces;

namespace snowcoreBlog.Backend.Infrastructure.RequestPersistence;

/// <summary>
/// TickerQ job: polls the claim store for interrupted requests — aged Persisted (claimed but
/// never executed), due RetryNeeded, Processing with expired lease — takes them over atomically
/// and replays them through <see cref="ReplayRequestDispatcher"/> (loopback re-dispatch, resume
/// from checkpoints). Replaces the former <c>PersistRequestRecoveryWorker</c> BackgroundService;
/// the cadence comes from the seeded cron ticker (<see cref="PersistRequestTickerQ"/>), which
/// derives the expression from <see cref="PersistRequestOptions.RecoveryPollInterval"/>.
/// <para>
/// Also guarantees construction of <see cref="StepCheckpointCoordinator"/> whenever TickerQ
/// builds this class (constructor dependency), so the static reference the woven step advices
/// read is published before the first recovery pass. Eager startup construction is additionally
/// enforced by <c>UseTickerQConfigurationAsync</c> resolving the coordinator before the host runs.
/// </para>
/// </summary>
public sealed class PersistRequestRecoveryJob(
    IServiceScopeFactory scopeFactory,
    IOptions<PersistRequestOptions> options,
    ILogger<PersistRequestRecoveryJob> logger) : ITickerFunction
{
    public async Task ExecuteAsync(TickerFunctionContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await RecoverDueAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Host shutdown — the next scheduled occurrence picks the pass back up.
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Recovery pass failed; retrying on next poll.");
        }
    }

    private async Task RecoverDueAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRequestPersistenceRepository>();
        var dispatcher = scope.ServiceProvider.GetRequiredService<ReplayRequestDispatcher>();
        var opts = options.Value;
        var now = DateTimeOffset.UtcNow;

        var candidates = await repository.GetRecoveryCandidatesAsync(now, opts.RecoverPersistedAfter, opts.RecoveryBatchSize, ct);
        foreach (var candidate in candidates)
        {
            if (ct.IsCancellationRequested)
            {
                return;
            }

            if (candidate.RetryCount >= opts.RetryBudget && candidate.Status != RequestPersistenceStatus.Processing)
            {
                await repository.MarkReplayFailureAsync(candidate.Id, "Retry budget exhausted.", ct);
                continue;
            }

            var takeoverNow = DateTimeOffset.UtcNow;
            if (!await repository.TryTakeOverAsync(candidate.Id, candidate.RetryCount, takeoverNow, opts.LeaseTtl, ct))
            {
                continue; // another instance won the takeover
            }

            ReplayOutcome outcome;
            try
            {
                outcome = await dispatcher.DispatchAsync(candidate, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Replay dispatch failed for request {RequestId}.", candidate.Id);
                outcome = new ReplayOutcome(RequestPersistenceStatus.RetryNeeded, null, null, ex.Message);
            }

            await ApplyOutcomeAsync(repository, opts, candidate, outcome, ct);
        }
    }

    private static async Task ApplyOutcomeAsync(
        IRequestPersistenceRepository repository,
        PersistRequestOptions opts,
        PersistedRequestEntity candidate,
        ReplayOutcome outcome,
        CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;

        if (outcome.Status == RequestPersistenceStatus.RetryNeeded)
        {
            var row = await repository.GetByIdAsync(candidate.Id, ct);
            if (row is default(PersistedRequestEntity))
            {
                return;
            }

            await repository.UpdateAsync(row with
            {
                Status = RequestPersistenceStatus.RetryNeeded,
                LeaseUntil = null,
                NextAttemptAt = opts.ComputeBackoff(row.RetryCount, now),
                Note = outcome.Note ?? outcome.ResponsePayload,
                UpdatedAt = now
            }, ct);
            return;
        }

        var finished = await repository.GetByIdAsync(candidate.Id, ct);
        if (finished is default(PersistedRequestEntity))
        {
            return;
        }

        var updated = finished with
        {
            Status = outcome.Status,
            ResponsePayload = outcome.ResponsePayload,
            ResponseStatusCode = outcome.StatusCode,
            LeaseUntil = null,
            NextAttemptAt = null,
            Note = outcome.Note,
            UpdatedAt = now
        };
        await repository.UpdateAsync(updated, ct);
        await PersistRequestRetentionPolicy.ApplyAsync(repository, opts, updated, now, ct);
    }
}
