using Fido2NetLib.Objects;

namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record ValidateAndCreateAssertion
{
    public required string Email { get; init; }

    public UserVerificationRequirement UserVerification { get; init; }
}