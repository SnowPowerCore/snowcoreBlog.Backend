using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Entities.Reader;
using snowcoreBlog.Backend.ReadersManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;

public class CreateNewReaderEntityStep(IReaderRepository readerRepository) : IStep<CreateReaderAccountDelegate, CreateReaderAccountContext, IResult<ReaderAccountCreationResultDto>>
{
    public async Task<IResult<ReaderAccountCreationResultDto>> InvokeAsync(CreateReaderAccountContext context, CreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var createUserForReaderAccountResult = context.GetFromData<SuccessResult<UserCreationResult>>(
            ReaderAccountUserConstants.CreateUserForReaderAccountResult);

        var newReaderEntity = await readerRepository
            .AddOrUpdateAsync(context.Request.ToEntity(createUserForReaderAccountResult!.Data.Id), token: token);

        if (newReaderEntity is not default(ReaderEntity))
        {
            context.SetDataWith(
                ReaderAccountConstants.CreateReaderAccountResult,
                Result.Success(new ReaderAccountCreationResultDto(newReaderEntity.Id)));

            return await next(context, token);
        }
        else
        {
            return CreateReaderAccountError<ReaderAccountCreationResultDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToCreateUpdateError);
        }
    }
}