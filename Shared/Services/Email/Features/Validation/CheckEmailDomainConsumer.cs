using System.Net.Mail;
using DnsClient;
using FluentValidation;
using MassTransit;
using MaybeResults;
using snowcoreBlog.Backend.Email.Core.Constants;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.Email.Features.Validation;

public class CheckEmailDomainConsumer : IConsumer<CheckEmailDomain>
{
    private readonly IValidator<CheckEmailDomain> _validator;
    private readonly LookupClient _lookupClient;

    public CheckEmailDomainConsumer(IValidator<CheckEmailDomain> validator)
    {
        _validator = validator;
        _lookupClient = new LookupClient(new LookupClientOptions { UseCache = true });
    }

    public async Task Consume(ConsumeContext<CheckEmailDomain> context)
    {
        var result = await _validator.ValidateAsync(context.Message, context.CancellationToken);
        if (!result.IsValid)
        {
            await context.RespondAsync(
                new DataResult<EmailDomainChecked>(
                    Errors: result.Errors.Select(e => new NoneDetail(e.PropertyName, e.ErrorMessage)).ToList()));
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
                    Errors: [new NoneDetail(context.Message.Email, EmailConstants.EmailDomainIsNotValid)]));
    }

    private async Task<bool> CheckDnsEntriesAsync(string domain)
    {
        try
        {
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var result = await _lookupClient.QueryAsync(
                domain, QueryType.MX, cancellationToken: cts.Token);
            return result.Answers.Any(record =>
                record.RecordType == DnsClient.Protocol.ResourceRecordType.MX);
        }
        catch (DnsResponseException)
        {
            return false;
        }
    }
}