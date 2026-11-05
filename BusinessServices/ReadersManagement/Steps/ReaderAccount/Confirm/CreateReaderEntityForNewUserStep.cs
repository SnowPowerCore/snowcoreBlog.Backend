using MassTransit;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Core.Contracts;
using snowcoreBlog.Backend.Core.Entities.Reader;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.Infrastructure.RequestPersistence;
using snowcoreBlog.Backend.Infrastructure.StepProgress;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Confirm;

[CheckpointedStep]
public class CreateReaderEntityForNewUserStep(IRequestClient<DeleteUser> deleteUserClient,
                                              IPublishEndpoint publishEndpoint,
                                              IReaderRepository readerRepository,
                                              ILogger<CreateReaderEntityForNewUserStep> logger) : IStep<ConfirmCreateReaderAccountDelegate, ConfirmCreateReaderAccountContext, IMaybe<ReaderAccountCreatedDto>>
{
    public async Task<IMaybe<ReaderAccountCreatedDto>> InvokeAsync(ConfirmCreateReaderAccountContext context, ConfirmCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        context.ReportStepProgress(ReaderAccountStepProgressCodes.CreatingReaderProfile);

        var createUserForReaderAccountResult = context.GetFromData<DataResult<UserCreationResult>>(
            ReaderAccountUserConstants.CreateUserForReaderAccountResult);

        var userCreationResult = createUserForReaderAccountResult!.Value!;

        // Exceptions are intentionally not compensated here: a transient failure surfaces as 5xx,
        // the persisted-request replay retries this step, and the still-existing IAM user is
        // exactly what the retry needs to finish the flow.
        var newReaderEntity = await readerRepository
            .AddOrUpdateAsync(userCreationResult.ToEntity(), token: token);
        if (newReaderEntity is not default(ReaderEntity))
        {
            var readerAccountCreated = new ReaderAccountCreated(newReaderEntity!.Id, newReaderEntity.NickName);

            context.SetDataWith(
                ReaderAccountConstants.CreateReaderAccountResult,
                Maybe.Create(readerAccountCreated));

            await publishEndpoint.Publish(readerAccountCreated, token);

            return await next(context, token);
        }
        else
        {
            // Deterministic failure: a replay would hit the same wall, so roll the identity
            // entity back instead of leaving an account without a reader profile.
            await RollbackUserCreationAsync(userCreationResult.Id, token);

            return CreateReaderAccountError<ReaderAccountCreatedDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToCreateUpdateError);
        }
    }

    private async Task RollbackUserCreationAsync(Guid userId, CancellationToken token)
    {
        try
        {
            await deleteUserClient.GetResponse<DataResult<Guid>>(
                new DeleteUser { UserId = userId }, token);
        }
        catch (Exception ex)
        {
            // Rethrow so the request fails as 5xx and the persisted-request replay retries this
            // step; the rollback itself is idempotent (already-deleted user responds success).
            logger.LogError(ex, "Unable to roll back IAM user {UserId} after reader entity creation failed.", userId);
            throw;
        }
    }
}