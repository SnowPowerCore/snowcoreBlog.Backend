using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Extensions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;

public class SendEmailToNewReaderAccountStep(IPublishEndpoint publishEndpoint) : IStep<CreateReaderAccountDelegate, CreateReaderAccountContext, IResult<ReaderAccountCreationResultDto>>
{
    public async Task<IResult<ReaderAccountCreationResultDto>> InvokeAsync(CreateReaderAccountContext context, CreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var sendTask = publishEndpoint.Publish(
            GenericEmailExtensions.ToGeneric(
                context.Request.Email,
                EmailConstants.ReaderAccountCreatedSubject,
                ""), token);
        var continueTask = next(context, token);
        await Task.WhenAll(sendTask, continueTask);
        return await continueTask;
    }
}