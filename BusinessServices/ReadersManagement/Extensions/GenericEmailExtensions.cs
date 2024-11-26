using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.Email.Core.Contracts;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions;

[Mapper]
public static partial class GenericEmailExtensions
{
    public static SendGenericEmail ToGeneric(
        string senderEmail, string subject, string? content = default) =>
        new()
        {
            SenderAddress = senderEmail,
            Subject = subject,
            Content = content
        };
}