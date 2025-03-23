namespace snowcoreBlog.Backend.Core.Constants;

public static class AuthCookieConstants
{
    public const string UserAccessTokenCookieName = ".DotNet.Application.User.SystemKey";
    public const string AdminAccessTokenCookieName = ".DotNet.Application.Admin.SystemKey";
    public const string UserRefreshTokenCookieName = ".DotNet.Application.User.SystemUpdateKey";
    public const string AdminRefreshTokenCookieName = ".DotNet.Application.Admin.SystemUpdateKey";
}