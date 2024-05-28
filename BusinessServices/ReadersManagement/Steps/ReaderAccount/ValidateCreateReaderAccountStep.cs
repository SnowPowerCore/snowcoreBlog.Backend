using FluentValidation;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Core;
using snowcoreBlog.PublicApi;

namespace snowcoreBlog.Backend.ReadersManagement;

public class ValidateCreateReaderAccountStep(IValidator<CreateReaderAccountDto> validator) : IStep<CreateReaderAccountDelegate, CreateReaderAccountContext>
{
    public async Task InvokeAsync(CreateReaderAccountContext context, CreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var result = await validator.ValidateAsync(context.Request, token);
        if (!result.IsValid)
        {
            context.SetDataWith(
                ReaderAccountConstants.CreateReaderAccountResult,
                new ValidationErrorResult<CreateReaderAccountDto>(result));
            return;
        }
        await next(context, token);
    }
}