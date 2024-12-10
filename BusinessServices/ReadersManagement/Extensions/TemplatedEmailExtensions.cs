using System.Diagnostics.CodeAnalysis;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.Email.Core.Options;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions;

public static partial class TemplatedEmailExtensions
{
    public static SendTemplatedEmail ToActivateCreatedTempUserEmail(
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
                [nameof(subject)] = subject,
                [nameof(preHeader)] = preHeader,
                ["userFirstName"] = readerAccountTempUserCreated.UserFirstName,
                ["verificationUrl"] = readerAccountTempUserCreated.VerificationUrl,
                ["verificationTokenUntilThatDate"] = readerAccountTempUserCreated.VerificationTokenUntilThatDate
            },
        };
}