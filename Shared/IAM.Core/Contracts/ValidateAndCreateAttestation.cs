using Fido2NetLib.Objects;

namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record ValidateAndCreateAttestation
{
    public required string Email { get; init; }

    public required string VerificationToken { get; init; }

    public AuthenticatorAttachment? AuthenticatorAttachment { get; init; }

    public AttestationConveyancePreference AttestationType { get; init; }

    public ResidentKeyRequirement ResidentKey { get; init; }

    public UserVerificationRequirement UserVerification { get; init; }
}