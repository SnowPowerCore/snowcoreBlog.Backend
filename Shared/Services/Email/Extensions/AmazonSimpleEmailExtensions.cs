using System.Text;
using Amazon.SimpleEmailV2.Model;
using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.Email.Core.Contracts;

namespace snowcoreBlog.Backend.Email.Extensions;

[Mapper]
public static partial class AmazonSimpleEmailExtensions
{
    [MapProperty(nameof(SendGenericEmail.SenderAddress), nameof(SendEmailRequest.FromEmailAddress))]
    [MapProperty(nameof(SendGenericEmail.Subject), nameof(@SendEmailRequest.Content.Simple.Subject.Data))]
    [MapProperty(nameof(SendGenericEmail.Content), nameof(@SendEmailRequest.Content.Simple.Body.Text.Data))]
    [MapProperty(nameof(SendGenericEmail.Content), nameof(@SendEmailRequest.Content.Simple.Body.Html.Data))]
    [MapValue(nameof(@SendEmailRequest.Content.Simple.Body.Text.Charset), Use = nameof(GetUtf8EncodingStr))]
    [MapValue(nameof(@SendEmailRequest.Content.Simple.Body.Html.Charset), Use = nameof(GetUtf8EncodingStr))]
    private static partial SendEmailRequest MapperToAmazonSimpleEmailRequest(this SendGenericEmail genericEmail);

    private static string GetUtf8EncodingStr() => Encoding.UTF8.WebName.ToUpper();

    public static SendEmailRequest ToAmazonSimpleEmailRequest(this SendGenericEmail genericEmail)
    {
        var request = MapperToAmazonSimpleEmailRequest(genericEmail);
        request.Destination ??= new();
        if (request.Destination.ToAddresses is not default(List<string>))
        {
            request.Destination.ToAddresses.Add(genericEmail.ReceiverAddress);
        }
        else
        {
            request.Destination.ToAddresses = [genericEmail.ReceiverAddress];
        }
        return request;
    }
}