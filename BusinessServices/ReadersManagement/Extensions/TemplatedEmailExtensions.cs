using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.Core.Contracts;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.Email.Core.Options;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions;

[Mapper]
public static partial class TemplatedEmailExtensions
{
    private static partial SendTemplatedEmail MapperToSendTemplatedEmail(
        this SendGridSenderAccountOptions senderOptions,
        string templateId,
        string receiverAddress);

    public static SendTemplatedEmail ToActivateCreatedTempUserEmail(
        ReaderAccountTempUserCreated readerAccountTempUserCreated,
        string templateId,
        SendGridSenderAccountOptions senderOptions,
        string receiverAddress, string subject, string preHeader = "")
    {
        var sendTemplatedEmail = MapperToSendTemplatedEmail(senderOptions, templateId, receiverAddress);
        sendTemplatedEmail = sendTemplatedEmail with
        {
            DynamicTemplateData = new Dictionary<string, string>()
            {
                [nameof(subject)] = subject,
                [nameof(preHeader)] = preHeader,
                ["userFirstName"] = readerAccountTempUserCreated.UserFirstName,
                ["verificationUrl"] = readerAccountTempUserCreated.VerificationUrl,
                ["verificationTokenUntilThatDate"] = readerAccountTempUserCreated.VerificationTokenUntilThatDate
            }
        };
        return sendTemplatedEmail;
    }
}