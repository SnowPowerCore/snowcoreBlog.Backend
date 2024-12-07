using System.Diagnostics.CodeAnalysis;
using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.Email.Core.Options;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions;

[Mapper]
public static partial class GenericEmailExtensions
{
    public static SendGenericEmail ToGeneric(
        [NotNull] SendGridSenderAccountOptions senderOptions,
        string receiverAddress, string subject, string? content = default) =>
        new()
        {
            SenderAddress = senderOptions.SenderAddress,
            SenderName = senderOptions.SenderName,
            ReceiverAddress = receiverAddress,
            Subject = subject,
            Content = content
        };
}