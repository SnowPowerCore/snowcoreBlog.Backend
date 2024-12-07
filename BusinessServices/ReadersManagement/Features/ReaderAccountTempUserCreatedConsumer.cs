using MassTransit;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.Email.Core.Options;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Extensions;

namespace snowcoreBlog.Backend.Email.Features.SendGrid;

public class ReaderAccountTempUserCreatedConsumer(IOptions<SendGridSenderAccountOptions> options,
                                                  IPublishEndpoint publishEndpoint,
                                                  IConfiguration configuration) : IConsumer<ReaderAccountTempUserCreated>
{
    public Task Consume(ConsumeContext<ReaderAccountTempUserCreated> context) =>
        publishEndpoint.Publish(
            TemplatedEmailExtensions.ToTemplated(
                context.Message,
                configuration.GetSection("SendGrid:DynamicTemplates")[EmailConstants.ReaderAccountTempUserCreatedTemplateId]!,
                options.Value,
                context.Message.UserEmail,
                EmailConstants.ReaderAccountTempUserCreatedSubject,
                EmailConstants.ReaderAccountTempUserCreatedPreHeader), context.CancellationToken);
}