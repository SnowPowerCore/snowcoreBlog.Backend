using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using MaybeResults;
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
                                              IReaderRepository readerRepository) : IStep<ConfirmCreateReaderAccountDelegate, ConfirmCreateReaderAccountContext, IMaybe<ReaderAccountCreatedDto>>
{
    public async Task<IMaybe<ReaderAccountCreatedDto>> InvokeAsync(ConfirmCreateReaderAccountContext context, ConfirmCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var createUserForReaderAccountResult = context.GetFromData<Some<UserCreationResult>>(
            ReaderAccountUserConstants.CreateTempUserForReaderAccountResult);

        var newReaderEntity = await readerRepository
            .AddOrUpdateAsync(createUserForReaderAccountResult!.Value.ToEntity(), token: token);
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
            return CreateReaderAccountError<ReaderAccountCreatedDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToCreateUpdateError);
        }
    }
}