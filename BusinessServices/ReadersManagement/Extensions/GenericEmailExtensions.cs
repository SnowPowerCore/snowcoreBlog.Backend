using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.Email.Core.Options;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions;

[Mapper]
public static partial class GenericEmailExtensions
{
    public static partial SendGenericEmail ToGeneric(
        SendGridSenderAccountOptions senderOptions,
        string receiverAddress, string subject, string? content = default);
}