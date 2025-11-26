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
            GenericEmailExtensions.ToGeneric(
                options.Value,
                context.Message.UserEmail,
                EmailConstants.ReaderAccountTempUserCreatedSubject,
                $"Welcome {context.Message.UserFirstName}, your activation link is: {context.Message.VerificationUrl}"), context.CancellationToken);
}