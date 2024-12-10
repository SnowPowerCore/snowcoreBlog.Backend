using FluentValidation;
using MassTransit;
using Results;
using SendGrid;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.Email.Extensions;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.Email.Features.SendGrid;

public class SendTemplatedEmailUsingSendGridConsumer(IValidator<SendTemplatedEmail> validator, ISendGridClient client) : IConsumer<SendTemplatedEmail>
{
    public async Task Consume(ConsumeContext<SendTemplatedEmail> context)
    {
        var result = await validator.ValidateAsync(context.Message, context.CancellationToken);
        if (!result.IsValid)
        {
            await context.RespondAsync(
                new DataResult<TemplatedEmailSent>(
                    Errors: result.Errors.Select(e => new ErrorResultDetail(e.PropertyName, e.ErrorMessage)).ToList()));
            return;
        }

        var message = context.Message.ToSendGrid();
        var response = await client.SendEmailAsync(message, context.CancellationToken);
        if (response.IsSuccessStatusCode)
            await context.Publish<TemplatedEmailSent>(new(), context.CancellationToken);
    }
}