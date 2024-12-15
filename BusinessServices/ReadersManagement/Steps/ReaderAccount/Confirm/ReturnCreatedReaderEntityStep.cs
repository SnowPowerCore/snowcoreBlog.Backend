using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;

public class ReturnCreatedReaderEntityStep() : IStep<ConfirmCreateReaderAccountDelegate, ConfirmCreateReaderAccountContext, IResult<ReaderAccountCreatedDto>>
{
    public Task<IResult<ReaderAccountCreatedDto>> InvokeAsync(ConfirmCreateReaderAccountContext context, ConfirmCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var readerAccountUserCreated = context.GetFromData<IResult<ReaderAccountUserCreated>>(
            ReaderAccountConstants.CreateReaderAccountUserResult);
        var readerAccountCreated = context.GetFromData<IResult<ReaderAccountCreated>>(
            ReaderAccountConstants.CreateReaderAccountResult);

        if (readerAccountUserCreated is SuccessResult<ReaderAccountUserCreated> successReaderAccountUserCreated
            && readerAccountCreated is SuccessResult<ReaderAccountCreated> successReaderAccountCreated)
        {
            return Task.FromResult(Result.Success(new ReaderAccountCreatedDto(
                successReaderAccountCreated.Data.Id,
                successReaderAccountUserCreated.Data.UserEmail)));
        }

        return Task.FromResult(CreateReaderAccountError<ReaderAccountCreatedDto>
            .Create(ReaderAccountConstants.ReaderAccountUnableToCreateUpdateError));
    }
}