using Fido2NetLib;

namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record LoginUser
{
    public required string Email { get; init; }

    public required string AssertionOptionsJson { get; init; }

    public required AuthenticatorAssertionRawResponse AuthenticatorAssertion { get; init; }
}