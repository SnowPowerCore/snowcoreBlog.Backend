namespace snowcoreBlog.Backend.Core.Constants;

public static class AuthCookieConstants
{
    public const string UserAccessTokenCookieName = ".DotNet.Application.User.SystemKey";
    public const string AdminAccessTokenCookieName = ".DotNet.Application.Admin.SystemKey";
    public const string UserRefreshTokenCookieName = ".DotNet.Application.User.SystemUpdateKey";
    public const string AdminRefreshTokenCookieName = ".DotNet.Application.Admin.SystemUpdateKey";

    // BitzArt.Blazor.Auth.Server persists the backend JWT pair in the browser
    // under these names; the backend-named auth cookies never reach the browser
    // when sign-in is completed through the Blazor host.
    public const string BitzArtAccessTokenCookieName = "AccessToken";
    public const string BitzArtRefreshTokenCookieName = "RefreshToken";
}