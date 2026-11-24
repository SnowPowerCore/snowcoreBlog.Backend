namespace snowcoreBlog.Backend.ReadersManagement.Core.Contracts;

public sealed record ReaderAccountCreated(
    Guid Id,
    string NickName);