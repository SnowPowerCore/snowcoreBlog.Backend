using SendGrid.Helpers.Mail;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.Email.Core.Models.Email;

namespace snowcoreBlog.Backend.Email.Extensions;

public static class SendGridExtensions
{
    public static SendGridMessage ToSendGrid(this SendGenericEmail genericEmail) =>
        new()
        {
            From = new EmailAddress(
                genericEmail.SenderAddress,
                genericEmail.SenderName ?? string.Empty),
            Subject = genericEmail.Subject,
            HtmlContent = genericEmail.Content
        };

    public static SendGridMessage ToSendGrid(this SendTemplatedEmail<ActivateCreatedTempUserData> templatedEmail) =>
        MailHelper.CreateSingleTemplateEmail(
            new EmailAddress(
                templatedEmail.SenderAddress,
                templatedEmail.SenderName ?? string.Empty),
            new EmailAddress(
                templatedEmail.ReceiverAddress,
                templatedEmail.ReceiverName ?? string.Empty),
            templatedEmail.TemplateId,
            templatedEmail.DynamicTemplateData);
}