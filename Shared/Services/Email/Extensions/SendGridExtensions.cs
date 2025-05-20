using Riok.Mapperly.Abstractions;
using SendGrid.Helpers.Mail;
using snowcoreBlog.Backend.Email.Core.Contracts;

namespace snowcoreBlog.Backend.Email.Extensions;

[Mapper]
public static partial class SendGridExtensions
{
    [MapProperty(nameof(SendGenericEmail.Content), nameof(SendGridMessage.HtmlContent))]
    [MapProperty(nameof(SendGenericEmail.SenderAddress), nameof(SendGridMessage.From.Email))]
    [MapProperty(nameof(SendGenericEmail.SenderName), nameof(SendGridMessage.From.Name))]
    public static partial SendGridMessage ToSendGrid(this SendGenericEmail genericEmail);

    public static SendGridMessage ToSendGrid(this SendTemplatedEmail templatedEmail) =>
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