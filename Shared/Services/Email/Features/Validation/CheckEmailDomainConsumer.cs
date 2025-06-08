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

namespace snowcoreBlog.Backend.Email.Features.Validation;

public class CheckEmailDomainConsumer : IConsumer<CheckEmailDomain>
{
    private readonly IValidator<CheckEmailDomain> _validator;
    private readonly HttpClient _httpClient;
    private readonly IConnectionMultiplexer _redis;

    private const string RedisDisposableDomainsKey = "DisposableEmailDomainsList";
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromHours(12);
    private static readonly string _fallbackUrl = "https://rawcdn.githack.com/disposable/disposable-email-domains/master/domains.json";

    public CheckEmailDomainConsumer(IValidator<CheckEmailDomain> validator, IConnectionMultiplexer redis)
    {
        _validator = validator;
        _httpClient = new HttpClient();
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
        var url = $"https://disposable.github.io/disposable-email-domains/cache/{prefix}.json";
        var response = await _httpClient.GetAsync(url);
        try
        {
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to fetch disposable domains from {url}. Status code: {response.StatusCode}");
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            // The JSON is a dictionary: hash -> info
            if (doc.RootElement.TryGetProperty(Convert.ToHexStringLower(hashBytes), out var info))
            {
                // Check if whitelisted property exists and is true
                if (info.TryGetProperty("whitelist", out var whitelistedProp) && whitelistedProp.GetBoolean())
                {
                    // Domain is whitelisted, treat as not disposable
                    return true;
                }
                // Found in disposable list and not whitelisted
                return false;
            }
            // Not found, not disposable
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
                    var fallbackResponse = await _httpClient.GetAsync(_fallbackUrl);
                    fallbackResponse.EnsureSuccessStatusCode();
                    fallbackJson = await fallbackResponse.Content.ReadAsStringAsync();
                    await db.StringSetAsync(RedisDisposableDomainsKey, fallbackJson, _cacheDuration);
                }
                catch
                {
                    return true;
                }
            }
            var items = JsonSerializer.Deserialize<List<string>?>(fallbackJson!);
            if (items is not default(List<string>))
                domains.AddRange(items);
            if (domains.Contains(cleanedDomain))
                return false;
            return true;
        }
    }
}