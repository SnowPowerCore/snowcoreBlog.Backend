using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.NickName;

public class ValidateNickNameWasNotTakenStep(IRequestClient<ValidateUserNickNameTaken> requestClient) : IStep<CheckNickNameNotTakenDelegate, CheckNickNameNotTakenContext, IResult<NickNameNotTakenCheckResultDto>>
{
    public async Task<IResult<NickNameNotTakenCheckResultDto>> InvokeAsync(CheckNickNameNotTakenContext context, CheckNickNameNotTakenDelegate next, CancellationToken token = default)
    {
        var result = await requestClient.GetResponse<DataResult<UserNickNameTakenValidationResult>>(
            new ValidateUserNickNameTaken() { NickName = context.NickName }, token);
        if (result.Message.IsSuccess)
        {
            if (result.Message.Value!.WasTaken)
            {
                return Result.Success<NickNameNotTakenCheckResultDto>(new(WasTaken: true));
            }
            else
            {
                return Result.Success<NickNameNotTakenCheckResultDto>(new(WasTaken: false));
            }
        }
        else
        {
            return NickNameError<NickNameNotTakenCheckResultDto>.Create(
                UserNickNameConstants.Failed, result.Message.Errors);
        }
    }
}