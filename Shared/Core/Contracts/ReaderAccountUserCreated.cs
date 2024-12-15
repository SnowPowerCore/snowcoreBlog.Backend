namespace snowcoreBlog.Backend.Core.Contracts;

public sealed record ReaderAccountUserCreated(
    Guid Id,
    string UserEmail);