using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.Core;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.PublicApi;

namespace snowcoreBlog.Backend.ReadersManagement;

public class CreateNewReaderEntityStep(IReaderRepository readerRepository) : IStep<CreateReaderAccountDelegate, CreateReaderAccountContext>
{
    public async Task InvokeAsync(CreateReaderAccountContext context, CreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var createUserForReaderAccountResult = context.GetFromData<SuccessResult<UserCreationResult>>(
                ReaderAccountConstants.CreateUserForReaderAccountResult);

        var newReaderEntity = await readerRepository
            .AddOrUpdateAsync(context.Request.ToEntity(createUserForReaderAccountResult.Data.Id), token: token);

        if (newReaderEntity is default(ReaderEntity))
        {
            context.SetDataWith(
                ReaderAccountConstants.CreateReaderAccountResult,
                CreateReaderAccountError<ReaderAccountCreationResultDto>.Create(
                    ReaderAccountConstants.ReaderAccountUnableToCreateUpdateError));
        }
        else
        {
            context.SetDataWith(
                ReaderAccountConstants.CreateReaderAccountResult,
                Result.Success(new ReaderAccountCreationResultDto(newReaderEntity.Id)));
            await next(context, token);
        }
    }
}