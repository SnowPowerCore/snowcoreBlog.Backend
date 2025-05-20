using Amazon.SimpleEmailV2.Model;
using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.Email.Core.Contracts;

namespace snowcoreBlog.Backend.Email.Extensions;

[Mapper]
public static partial class AmazonSimpleEmailExtensions
{
    [MapProperty(nameof(SendGenericEmail.SenderAddress), nameof(SendEmailRequest.FromEmailAddress))]
    [MapProperty(nameof(SendGenericEmail.SenderAddress), nameof(SendEmailRequest.Destination))]
    public static partial SendEmailRequest ToAmazonSimpleEmailRequest(this SendGenericEmail genericEmail);
    // new()
    // {
    //     Source = model.SenderAddress,
    //     Destination = new Destination
    //     {
    //         ToAddresses = new List<string> { model.NotifiedEntityAddress }
    //     },
    //     Message = new Message
    //     {
    //         Subject = new Content(model.Subject),
    //         Body = new Body
    //         {
    //             Html = new Content
    //             {
    //                 Charset = "UTF-8",
    //                 Data = model.Content
    //             }
    //         }
    //     }
    // };
}
