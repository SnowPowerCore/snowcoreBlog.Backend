using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FluentValidation;
using MassTransit;
using MaybeResults;
using snowcoreBlog.Backend.Email.Core.Constants;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.PublicApi.Utilities.DataResult;
using StackExchange.Redis;
using Apizr;
using snowcoreBlog.Backend.Email.Api;

namespace snowcoreBlog.Backend.Email.Features.Validation;

public class CheckEmailDomainConsumer : IConsumer<CheckEmailDomain>
{
    private readonly IValidator<CheckEmailDomain> _validator;
    private readonly IApizrManager<IEmailDisposableApi> _emailDisposableApiManager;
    private readonly IApizrManager<IStaticEmailDisposableApi> _staticEmailDisposableApiManager;
    private readonly IConnectionMultiplexer _redis;

    private const string RedisDisposableDomainsKey = "DisposableEmailDomainsList";
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromHours(12);

    public CheckEmailDomainConsumer(IValidator<CheckEmailDomain> validator,
                                    IApizrManager<IEmailDisposableApi> emailDisposableApiManager,
                                    IApizrManager<IStaticEmailDisposableApi> staticEmailDisposableApiManager,
                                    IConnectionMultiplexer redis)
    {
        _validator = validator;
        _emailDisposableApiManager = emailDisposableApiManager;
        _staticEmailDisposableApiManager = staticEmailDisposableApiManager;
        _redis = redis;
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
        var isEmailDomain = await IsDomainNotDisposableAsync(host);
        if (isEmailDomain)
            await context.RespondAsync(new DataResult<EmailDomainChecked>(new()));
        else
            await context.RespondAsync(
                    new DataResult<EmailDomainChecked>(
                        Errors: [new NoneDetail(context.Message.Email, EmailConstants.EmailDomainIsNotValid)]));
    }

    private async Task<bool> IsDomainNotDisposableAsync(string input)
    {
        var cleanedDomain = input.Replace(" ", string.Empty).ToLowerInvariant();
        var hashBytes = SHA1.HashData(Encoding.UTF8.GetBytes(cleanedDomain));
        var prefix = Convert.ToHexStringLower(hashBytes).Substring(0, 2);
        try
        {
            var response = await _emailDisposableApiManager.ExecuteAsync((opt, api) => api.GetDisposableDomainsAsync(prefix, opt));
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to fetch disposable domains from prefix {prefix}. Status code: {response.StatusCode}");
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty(Convert.ToHexStringLower(hashBytes), out var info))
            {
                if (info.TryGetProperty("whitelist", out var whitelistedProp) && whitelistedProp.GetBoolean())
                {
                    return true;
                }
                return false;
            }
            return true;
        }
        catch
        {
            // Fallback: check against Redis-cached disposable domains list
            var db = _redis.GetDatabase();
            var fallbackJson = await db.StringGetAsync(RedisDisposableDomainsKey);
            List<string>? domains = [];
            if (string.IsNullOrEmpty(fallbackJson))
            {
                try
                {
                    var fallbackResponse = await _staticEmailDisposableApiManager.ExecuteAsync((opt, api) => api.GetFallbackDomainsAsync(opt));
                    fallbackResponse.EnsureSuccessStatusCode();
                    fallbackJson = await fallbackResponse.Content.ReadAsStringAsync();
                    await db.StringSetAsync(RedisDisposableDomainsKey, fallbackJson, _cacheDuration);
                }
                catch
                {
                    return true;
                }
            }
            var items = JsonSerializer.Deserialize<List<string>>(fallbackJson!);
            if (items is not default(List<string>))
                domains.AddRange(items);
            if (domains.Contains(cleanedDomain))
                return false;
            return true;
        }
    }
}