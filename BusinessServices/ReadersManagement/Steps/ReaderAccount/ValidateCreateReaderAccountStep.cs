using FluentValidation;
using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.Core;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;

public class ValidateCreateReaderAccountStep(IValidator<CreateReaderAccountDto> validator) : IStep<CreateReaderAccountDelegate, CreateReaderAccountContext, IResult<ReaderAccountCreationResultDto>>
{
    public async Task<IResult<ReaderAccountCreationResultDto>> InvokeAsync(CreateReaderAccountContext context, CreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var result = await validator.ValidateAsync(context.Request, token);
        if (result.IsValid)
        {
            return await next(context, token);
        }
        else
        {
            return new ValidationErrorResult<ReaderAccountCreationResultDto>(result);
        }
    }
}