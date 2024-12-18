using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.Core.Contracts;
using snowcoreBlog.Backend.Core.Entities.Reader;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;

public class CreateReaderEntityForNewUserStep(IPublishEndpoint publishEndpoint,
                                              IReaderRepository readerRepository) : IStep<ConfirmCreateReaderAccountDelegate, ConfirmCreateReaderAccountContext, IResult<ReaderAccountCreatedDto>>
{
    public async Task<IResult<ReaderAccountCreatedDto>> InvokeAsync(ConfirmCreateReaderAccountContext context, ConfirmCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var createUserForReaderAccountResult = context.GetFromData<SuccessResult<UserCreationResult>>(
            ReaderAccountUserConstants.CreateTempUserForReaderAccountResult);

        var newReaderEntity = await readerRepository
            .AddOrUpdateAsync(createUserForReaderAccountResult!.Data.ToEntity(), token: token);
        if (newReaderEntity is not default(ReaderEntity))
        {
            var readerAccountCreated = new ReaderAccountCreated(newReaderEntity!.Id, newReaderEntity.NickName);

            context.SetDataWith(
                ReaderAccountConstants.CreateReaderAccountResult,
                Result.Success(readerAccountCreated));

            await publishEndpoint.Publish(readerAccountCreated, token);

            return await next(context, token);
        }
        else
        {
            return CreateReaderAccountError<ReaderAccountCreatedDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToCreateUpdateError);
        }
    }
}