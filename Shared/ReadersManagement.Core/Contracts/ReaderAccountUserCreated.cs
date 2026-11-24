namespace snowcoreBlog.Backend.ReadersManagement.Core.Contracts;

public sealed record ReaderAccountUserCreated(
    Guid Id,
    string UserEmail);