using System.Security.Cryptography;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Delegates;
using snowcoreBlog.Backend.UrlShortener.Models;

namespace snowcoreBlog.Backend.UrlShortener.Steps;

public sealed class GenerateCodeStep : IStep<CreateShortUrlDelegate, CreateShortUrlContext, IMaybe<CreateShortUrlOperationResult>>
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

    public Task<IMaybe<CreateShortUrlOperationResult>> InvokeAsync(CreateShortUrlContext context, CreateShortUrlDelegate next, CancellationToken token = default)
    {
        context.Code = string.IsNullOrWhiteSpace(context.Request.CustomCode)
            ? GenerateCode(7)
            : context.Request.CustomCode;

        return next(context, token);
    }
}