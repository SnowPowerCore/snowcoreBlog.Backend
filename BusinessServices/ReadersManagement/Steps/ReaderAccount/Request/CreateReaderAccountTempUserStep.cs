﻿using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;

public class CreateReaderAccountTempUserStep(IRequestClient<CreateTempUser> client,
                                             IPublishEndpoint publishEndpoint) : IStep<RequestCreateReaderAccountDelegate, RequestCreateReaderAccountContext, IResult<RequestReaderAccountCreationResultDto>>
{
    public async Task<IResult<RequestReaderAccountCreationResultDto>> InvokeAsync(RequestCreateReaderAccountContext context, RequestCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var response = await client.GetResponse<DataResult<TempUserCreationResult>>(
            context.CreateRequest.ToCreateTempUser(), token);
        if (response.Message.IsSuccess)
        {
            var responseObj = response!.Message.Value;

            await publishEndpoint.Publish<ReaderAccountTempUserCreated>(
                new(responseObj!.FirstName,
                    responseObj.Email,
                    responseObj.VerificationToken + responseObj.Email,
                    responseObj.VerificationTokenExpirationDate.ToString()), token);

            return Result.Success<RequestReaderAccountCreationResultDto>(new(responseObj.Id));
        }
        else
        {
            return CreateUserForReaderAccountError<RequestReaderAccountCreationResultDto>.Create(
                ReaderAccountUserConstants.TempUserForReaderAccountUnableToCreateUpdateError, response.Message.Errors);
        }
    }
}