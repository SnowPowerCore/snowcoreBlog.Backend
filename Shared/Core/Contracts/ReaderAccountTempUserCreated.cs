namespace snowcoreBlog.Backend.Core.Contracts;

public sealed record ReaderAccountTempUserCreated(
    string UserFirstName,
    string UserEmail,
    string VerificationUrl,
    string VerificationTokenUntilThatDate);