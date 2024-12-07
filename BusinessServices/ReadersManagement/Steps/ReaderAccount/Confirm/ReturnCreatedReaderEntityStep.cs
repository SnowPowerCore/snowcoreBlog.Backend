using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;

public class ReturnCreatedReaderEntityStep() : IStep<RequestCreateReaderAccountDelegate, RequestCreateReaderAccountContext, IResult<RequestReaderAccountCreationResultDto>>
{
    public Task<IResult<RequestReaderAccountCreationResultDto>> InvokeAsync(RequestCreateReaderAccountContext context, RequestCreateReaderAccountDelegate next, CancellationToken token = default) =>
        Task.FromResult(context.GetFromData<IResult<RequestReaderAccountCreationResultDto>>(
            ReaderAccountConstants.CreateReaderAccountResult))!;
}