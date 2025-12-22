namespace snowcoreBlog.Backend.ReadersManagement.Constants;

public static class ReaderRefreshTokenConstants
{
    public const string RefreshTokenKeyPrefix = "reader-refresh:";
    public const string RefreshTokenLockKeyPrefix = "reader-refresh-lock:";
    public const string UserCurrentRefreshTokenKeyPrefix = "reader-user-refresh:";

    public static string RefreshTokenKey(string refreshToken) => $"{RefreshTokenKeyPrefix}{refreshToken}";

    public static string RefreshTokenLockKey(string refreshToken) => $"{RefreshTokenLockKeyPrefix}{refreshToken}";

    public static string UserCurrentRefreshTokenKey(Guid userId) => $"{UserCurrentRefreshTokenKeyPrefix}{userId:N}";
}
