using MassTransit;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Email.Core.Constants;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Extensions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Request;

public class ValidateReaderAccountEmailDomainStep(IRequestClient<CheckEmailDomain> requestClient) : IStep<RequestCreateReaderAccountDelegate, RequestCreateReaderAccountContext, IMaybe<RequestReaderAccountCreationResultDto>>
{
    public async Task<IMaybe<RequestReaderAccountCreationResultDto>> InvokeAsync(RequestCreateReaderAccountContext context, RequestCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var result = await requestClient.GetResponse<DataResult<EmailDomainChecked>>(
            context.CreateRequest.ToCheckEmailDomain(), token);
        if (result.Message.IsSuccess)
        {
            return await next(context, token);
        }
        else
        {
            return CreateUserForReaderAccountError<RequestReaderAccountCreationResultDto>.Create(
                EmailConstants.EmailDomainIsNotValid, result.Message.Errors);
        }
    }
}