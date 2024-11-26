using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.Email.Core.Constants;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Extensions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;

public class ValidateReaderAccountEmailDomainStep(IRequestClient<CheckEmailDomain> requestClient) : IStep<CreateReaderAccountDelegate, CreateReaderAccountContext, IResult<ReaderAccountCreationResultDto>>
{
    public async Task<IResult<ReaderAccountCreationResultDto>> InvokeAsync(CreateReaderAccountContext context, CreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var result = await requestClient.GetResponse<DataResult<EmailDomainChecked>>(context.Request.ToCheckEmailDomain());
        if (result.Message.IsSuccess)
        {
            return await next(context, token);
        }
        else
        {
            return CreateUserForReaderAccountError<ReaderAccountCreationResultDto>.Create(
                EmailConstants.EmailDomainIsNotValid, result.Message.Errors);
        }
    }
}