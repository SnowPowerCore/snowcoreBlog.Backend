using MassTransit;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.AdminsManagement.Options;
using snowcoreBlog.Backend.Email.Core.Contracts;

namespace snowcoreBlog.Backend.AdminsManagement.Services;

/// <summary>
/// Publishes admin invite/login emails onto the bus (backend-email consumes
/// <see cref="SendGenericEmail"/>; sender identity comes from this module's
/// Integrations:Email:SenderAccount config, mirroring the readers flow).
/// </summary>
public sealed class AdminEmailPublisher(IPublishEndpoint publishEndpoint, IOptions<AdminEmailSenderOptions> senderOptions)
{
    public Task SendAsync(string receiverAddress, string subject, string content, CancellationToken ct = default)
        => publishEndpoint.Publish(new SendGenericEmail
        {
            SenderAddress = senderOptions.Value.SenderAddress,
            SenderName = senderOptions.Value.SenderName,
            ReceiverAddress = receiverAddress,
            Subject = subject,
            Content = content,
        }, ct);
}
