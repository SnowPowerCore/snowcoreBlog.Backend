using SendGrid.Helpers.Mail;
using snowcoreBlog.Backend.Email.Core.Contracts;

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
}