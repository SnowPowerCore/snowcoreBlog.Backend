using System.Security.Cryptography;

namespace snowcoreBlog.Backend.Infrastructure.Extensions;

public static class StringExtensions
{
    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789#!?*.";

    public static string RandomString(int length) =>
        RandomNumberGenerator.GetString(Chars, length);
}