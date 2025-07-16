using Amazon.SimpleEmailV2;
using FluentValidation;
using MassTransit;
using MaybeResults;
using snowcoreBlog.Backend.Email.Constants;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.Email.Extensions;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.Email.Features.AmazonSimpleEmail;

public class SendGenericEmailUsingAmazonSimpleEmailConsumer(IValidator<SendGenericEmail> validator,
                                                            IAmazonSimpleEmailServiceV2 simpleEmail) : IConsumer<SendGenericEmail>
{
    public async Task Consume(ConsumeContext<SendGenericEmail> context)
    {
        var result = await validator.ValidateAsync(context.Message, context.CancellationToken);
        if (!result.IsValid)
        {
            await context.RespondAsync(
                new DataResult<GenericEmailSent>(
                    Errors: result.Errors.Select(e => new NoneDetail(e.PropertyName, e.ErrorMessage)).ToList()));
            return;
        }

        var sendResponse = await simpleEmail.SendEmailAsync(
            context.Message.ToAmazonSimpleEmailRequest(), context.CancellationToken);
        if (string.IsNullOrWhiteSpace(sendResponse?.MessageId))
        {
            await context.RespondAsync(
                new DataResult<GenericEmailSent>(
                    Errors: [new(nameof(sendResponse.MessageId), EmailConstants.EmailWasNotSent)]));
            return;
        }
        
        await context.Publish<GenericEmailSent>(new(), context.CancellationToken);
    }
}