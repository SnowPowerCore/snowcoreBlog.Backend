using System.Security.Cryptography;
using System.Text;

namespace snowcoreBlog.Backend.AdminsManagement.Services;

public static class AdminTokenUtilities
{
    public static string Sha256Hex(string value)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    public static string NewToken()
        => Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
}