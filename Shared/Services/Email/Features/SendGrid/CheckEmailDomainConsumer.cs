using System.Net.Mail;
using DnsClient;
using FluentValidation;
using MassTransit;
using Results;
using snowcoreBlog.Backend.Email.Core.Constants;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.Email.Features.SendGrid;

public class CheckEmailDomainConsumer(IValidator<CheckEmailDomain> validator) : IConsumer<CheckEmailDomain>
{
    public async Task Consume(ConsumeContext<CheckEmailDomain> context)
    {
        var result = await validator.ValidateAsync(context.Message, context.CancellationToken);
        if (!result.IsValid)
        {
            await context.RespondAsync(
                new DataResult<EmailDomainChecked>(
                    Errors: result.Errors.Select(e => new ErrorResultDetail(e.PropertyName, e.ErrorMessage)).ToList()));
            return;
        }

        var mailAddress = new MailAddress(context.Message.Email);
        var host = mailAddress.Host;
        var isEmailDomain = await CheckDnsEntriesAsync(host);
        if (isEmailDomain)
            await context.RespondAsync(
                new DataResult<EmailDomainChecked>(new()));
        else await context.RespondAsync(
                new DataResult<EmailDomainChecked>(
                    Errors: [new ErrorResultDetail(context.Message.Email, EmailConstants.EmailDomainIsNotValid)]));
    }

    private static async Task<bool> CheckDnsEntriesAsync(string domain)
    {
        try
        {
            var lookup = new LookupClient();
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var result = await lookup.QueryAsync(domain, QueryType.ANY, cancellationToken: cts.Token);
            return result.Answers.Where(record =>
                record.RecordType == DnsClient.Protocol.ResourceRecordType.A ||
                record.RecordType == DnsClient.Protocol.ResourceRecordType.AAAA ||
                record.RecordType == DnsClient.Protocol.ResourceRecordType.MX).Any();
        }
        catch (DnsResponseException)
        {
            return false;
        }
    }
}