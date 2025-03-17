using FluentValidation;
using MassTransit;
using ntfy;
using MaybeResults;
using snowcoreBlog.Backend.Push.Core.Contracts;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.Push.Features.Ntfy;

public class SendPushUsingNtfyConsumer(IValidator<SendGenericPush> validator,
                                       Client client) : IConsumer<SendGenericPush>
{
    public async Task Consume(ConsumeContext<SendGenericPush> context)
    {
        var result = await validator.ValidateAsync(context.Message, context.CancellationToken);
        if (!result.IsValid)
        {
            await context.RespondAsync(
                new DataResult<GenericPushSent>(
                    Errors: result.Errors.Select(e => new NoneDetail(e.PropertyName, e.ErrorMessage)).ToList()));
            return;
        }

        var message = context.Message.ToSendGrid();
        message.AddTo(new EmailAddress(context.Message.NotifiedEntityAddress, context.Message.NotifiedEntityName));
        var response = await client.SendEmailAsync(message, context.CancellationToken);
        if (response.IsSuccessStatusCode)
            await context.Publish<GenericPushSent>(new(), context.CancellationToken);
    }
}