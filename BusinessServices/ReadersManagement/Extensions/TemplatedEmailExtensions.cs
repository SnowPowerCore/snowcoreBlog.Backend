using System.Diagnostics.CodeAnalysis;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.Email.Core.Models.Email;
using snowcoreBlog.Backend.Email.Core.Options;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions;

public static partial class TemplatedEmailExtensions
{
    public static SendTemplatedEmail<ActivateCreatedTempUserData> ToActivateCreatedTempUserEmail(
        [NotNull] ReaderAccountTempUserCreated readerAccountTempUserCreated,
        [NotNull] string templateId,
        [NotNull] SendGridSenderAccountOptions senderOptions,
        [NotNull] string receiverAddress, [NotNull] string subject, string preHeader = "") =>
        new()
        {
            SenderAddress = senderOptions.SenderAddress,
            SenderName = senderOptions.SenderName,
            ReceiverAddress = receiverAddress,
            TemplateId = templateId,
            DynamicTemplateData = new()
            {
                Subject = subject,
                PreHeader = preHeader,
                UserFirstName = readerAccountTempUserCreated.UserFirstName,
                VerificationUrl = readerAccountTempUserCreated.VerificationUrl,
                VerificationTokenUntilThatDate = readerAccountTempUserCreated.VerificationTokenUntilThatDate,
            },
        };
}