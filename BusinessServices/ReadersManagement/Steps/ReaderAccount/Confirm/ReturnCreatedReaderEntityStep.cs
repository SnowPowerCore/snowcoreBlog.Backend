using MinimalStepifiedSystem.Interfaces;
using MaybeResults;
using snowcoreBlog.Backend.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;

public class ReturnCreatedReaderEntityStep() : IStep<ConfirmCreateReaderAccountDelegate, ConfirmCreateReaderAccountContext, IMaybe<ReaderAccountCreatedDto>>
{
    public Task<IMaybe<ReaderAccountCreatedDto>> InvokeAsync(ConfirmCreateReaderAccountContext context, ConfirmCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var readerAccountUserCreated = context.GetFromData<IMaybe<ReaderAccountUserCreated>>(
            ReaderAccountConstants.CreateReaderAccountUserResult);
        var readerAccountCreated = context.GetFromData<IMaybe<ReaderAccountCreated>>(
            ReaderAccountConstants.CreateReaderAccountResult);

        if (readerAccountUserCreated is Some<ReaderAccountUserCreated> successReaderAccountUserCreated
            && readerAccountCreated is Some<ReaderAccountCreated> successReaderAccountCreated)
        {
            return Task.FromResult(Maybe.Create(new ReaderAccountCreatedDto(
                successReaderAccountCreated.Value.Id,
                successReaderAccountUserCreated.Value.UserEmail)));
        }

        return Task.FromResult(CreateReaderAccountError<ReaderAccountCreatedDto>
            .Create(ReaderAccountConstants.ReaderAccountUnableToCreateUpdateError));
    }
}