using System.Buffers;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Apizr;
using FluentValidation;
using MassTransit;
using MaybeResults;
using snowcoreBlog.Backend.Email.Api;
using snowcoreBlog.Backend.Email.Core.Constants;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.PublicApi.Utilities.DataResult;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.Email.Features.Validation;

public class CheckEmailDomainConsumer(IValidator<CheckEmailDomain> validator,
                                IApizrManager<IEmailDisposableApi> emailDisposableApiManager,
                                IApizrManager<IStaticEmailDisposableApi> staticEmailDisposableApiManager,
                                IConnectionMultiplexer redis) : IConsumer<CheckEmailDomain>
{
    private const string RedisDisposableDomainsKey = "DisposableEmailDomainsList";
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromHours(12);

    public async Task Consume(ConsumeContext<CheckEmailDomain> context)
    {
        var result = await validator.ValidateAsync(context.Message, context.CancellationToken);
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
        Span<byte> hash = stackalloc byte[20];
        string hashHex;
        string prefix;

        byte[]? rented = null;
        try
        {
            ReadOnlySpan<char> domainSpan = cleanedDomain.AsSpan();
            int byteCount = Encoding.UTF8.GetByteCount(domainSpan);

            Span<byte> utf8Bytes = byteCount <= 256
                ? stackalloc byte[byteCount]
                : (rented = ArrayPool<byte>.Shared.Rent(byteCount));

            int bytesWritten = Encoding.UTF8.GetBytes(domainSpan, utf8Bytes);
            if (!SHA1.TryHashData(utf8Bytes.Slice(0, bytesWritten), hash, out int hashBytesWritten) || hashBytesWritten != hash.Length)
                throw new CryptographicException("Unable to compute SHA1 hash.");

            hashHex = Convert.ToHexStringLower(hash);
            prefix = string.Create(2, hash[0], static (dest, b) =>
            {
                const string Hex = "0123456789abcdef";
                dest[0] = Hex[b >> 4];
                dest[1] = Hex[b & 0xF];
            });
        }
        finally
        {
            if (rented is not null)
                ArrayPool<byte>.Shared.Return(rented, clearArray: true);
        }

        try
        {
            var response = await emailDisposableApiManager.ExecuteAsync((opt, api) => api.GetDisposableDomainsAsync(prefix, opt));
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to fetch disposable domains from prefix {prefix}. Status code: {response.StatusCode}");
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty(hashHex, out var info))
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
            var db = redis.GetDatabase();
            var fallbackJson = await db.StringGetAsync(RedisDisposableDomainsKey);
            List<string>? domains = [];
            if (string.IsNullOrWhiteSpace(fallbackJson))
            {
                try
                {
                    var fallbackResponse = await staticEmailDisposableApiManager.ExecuteAsync((opt, api) => api.GetFallbackDomainsAsync(opt));
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