using System.Security.Cryptography;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.UrlShortener.Steps.Create;

public sealed class GenerateCodeStep : IStep<CreateShortUrlDelegate, CreateShortUrlContext, IMaybe<CreateShortUrlResult>>
{
    private static readonly char[] _alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

    private static string GenerateCode(int length)
    {
        var bytes = RandomNumberGenerator.GetBytes(length);
        var sb = new System.Text.StringBuilder(length);
        for (var i = 0; i < length; i++)
        {
            var idx = bytes[i] % _alphabet.Length;
            sb.Append(_alphabet[idx]);
        }

        return sb.ToString();
    }

    public Task<IMaybe<CreateShortUrlResult>> InvokeAsync(CreateShortUrlContext context, CreateShortUrlDelegate next, CancellationToken token = default)
    {
        context.Code = string.IsNullOrWhiteSpace(context.Request.CustomCode)
            ? GenerateCode(7)
            : context.Request.CustomCode;

        return next(context, token);
    }
}