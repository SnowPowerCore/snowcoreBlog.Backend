using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.Core.Entities.Reader;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;

public class CreateNewReaderEntityStep(IPublishEndpoint publishEndpoint,
                                       IReaderRepository readerRepository) : IStep<RequestCreateReaderAccountDelegate, RequestCreateReaderAccountContext, IResult<RequestReaderAccountCreationResultDto>>
{
    public async Task<IResult<RequestReaderAccountCreationResultDto>> InvokeAsync(RequestCreateReaderAccountContext context, RequestCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var createUserForReaderAccountResult = context.GetFromData<SuccessResult<UserCreationResult>>(
            ReaderAccountUserConstants.CreateTempUserForReaderAccountResult);

        var newReaderEntity = await readerRepository
            .AddOrUpdateAsync(context.Request.ToEntity(createUserForReaderAccountResult!.Data.Id), token: token);

        if (newReaderEntity is not default(ReaderEntity))
        {
            context.SetDataWith(
                ReaderAccountConstants.CreateReaderAccountResult,
                Result.Success(new RequestReaderAccountCreationResultDto(newReaderEntity.Id)));

            return await next(context, token);
        }
        else
        {
            return CreateReaderAccountError<RequestReaderAccountCreationResultDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToCreateUpdateError);
        }
    }
}