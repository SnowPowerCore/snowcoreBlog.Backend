namespace snowcoreBlog.Backend.ReadersManagement.Core.Contracts;

public sealed record ReaderAccountUserLoggedIn(
    Guid Id, string Email);