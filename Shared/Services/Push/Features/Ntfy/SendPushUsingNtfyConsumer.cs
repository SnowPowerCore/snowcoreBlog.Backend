using FluentValidation;
using MassTransit;
using MaybeResults;
using NtfyCator;
using NtfyCator.Messages;
using snowcoreBlog.Backend.Push.Core.Contracts;
using snowcoreBlog.Backend.Push.Extensions;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.Push.Features.Ntfy;

public class SendPushUsingNtfyConsumer(IValidator<SendGenericPush> validator,
                                       INotificator notificator) : IConsumer<SendGenericPush>
{
    public async Task Consume(ConsumeContext<SendGenericPush> context)
    {
        var sendMessage = context.Message;
        var result = await validator.ValidateAsync(sendMessage, context.CancellationToken);
        if (!result.IsValid)
        {
            await context.RespondAsync(
                new DataResult<GenericPushSent>(
                    Errors: result.Errors.Select(e => new NoneDetail(e.PropertyName, e.ErrorMessage)).ToList()));
            return;
        }

        var message = new NtfyMessageBuilder(sendMessage.Topic)
            .WithTitle(sendMessage.Subject)
            .WithPriority(sendMessage.Priority.ToNtfyPriority());
        
        if (!string.IsNullOrEmpty(sendMessage.Content))
            message = message.WithBody(sendMessage.Content, sendMessage.IsContentMarkdown);
        
        if (!string.IsNullOrEmpty(sendMessage.Email))
            message = message.WithEmail(sendMessage.Email);
        
        if (!string.IsNullOrEmpty(sendMessage.PhoneNumber))
            message = message.WithPhoneNumber(sendMessage.PhoneNumber);
        
        if (!string.IsNullOrEmpty(sendMessage.IconUri))
            message = message.WithIconUri(sendMessage.IconUri);
        
        if (!string.IsNullOrEmpty(sendMessage.ClickUri))
            message = message.WithIconUri(sendMessage.ClickUri);
        
        if (sendMessage.Tags.Count > 0)
            message = message.WithTags(sendMessage.Tags);
        
        if (sendMessage.Attachment is not default(NotificationAttachment))
            message = message.WithAttachment(sendMessage.Attachment.Uri, sendMessage.Attachment.Name);
        
        if (sendMessage.Actions.Count > 0)
            message = message.WithActions(sendMessage.Actions.Select(x => x.ToNtfyAction()).ToArray());
        
        var notifyTask = notificator.NotifyAsync(message.Build(), context.CancellationToken);
        var responseTask = context.Publish<GenericPushSent>(new(), context.CancellationToken);

        await Task.WhenAll(notifyTask, responseTask);
    }
}