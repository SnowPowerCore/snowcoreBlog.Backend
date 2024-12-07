using FluentValidation;
using MassTransit;
using Results;
using SendGrid;
using SendGrid.Helpers.Mail;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.Email.Extensions;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.Email.Features.SendGrid;

public class SendGenericEmailUsingSendGridConsumer(IValidator<SendGenericEmail> validator,
                                                   ISendGridClient client) : IConsumer<SendGenericEmail>
{
    public async Task Consume(ConsumeContext<SendGenericEmail> context)
    {
        var result = await validator.ValidateAsync(context.Message, context.CancellationToken);
        if (!result.IsValid)
        {
            await context.RespondAsync(
                new DataResult<GenericEmailSent>(
                    Errors: result.Errors.Select(e => new ErrorResultDetail(e.PropertyName, e.ErrorMessage)).ToList()));
            return;
        }

        var message = context.Message.ToSendGrid();
        message.AddTo(new EmailAddress(context.Message.ReceiverAddress, context.Message.ReceiverName));
        var response = await client.SendEmailAsync(message, context.CancellationToken);
        if (response.IsSuccessStatusCode)
            await context.Publish<GenericEmailSent>(new(), context.CancellationToken);
    }
}