using MassTransit;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Core.Contracts;
using snowcoreBlog.Backend.Email.Core.Options;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Extensions;

namespace snowcoreBlog.Backend.ReadersManagement.Features;

public class ReaderAccountTempUserCreatedConsumer(IOptions<SendGridSenderAccountOptions> options,
                                                  IConfiguration configuration) : IConsumer<ReaderAccountTempUserCreated>
{
    public Task Consume(ConsumeContext<ReaderAccountTempUserCreated> context) =>
        context.Publish(
            TemplatedEmailExtensions.ToActivateCreatedTempUserEmail(
                context.Message,
                configuration.GetSection("SendGrid:DynamicTemplates")[EmailConstants.ReaderAccountTempUserCreatedTemplateId]!,
                options.Value,
                context.Message.UserEmail,
                EmailConstants.ReaderAccountTempUserCreatedSubject,
                EmailConstants.ReaderAccountTempUserCreatedPreHeader), context.CancellationToken);
}