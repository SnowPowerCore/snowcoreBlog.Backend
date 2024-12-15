using Fido2NetLib;

namespace snowcoreBlog.Backend.IAM.Core.Contracts;

public sealed record CreateUser
{
    public required string Email { get; init; }

    public required string TempUserVerificationToken { get; init; }

    public required string AttestationOptionsJson { get; init; }

    public required AuthenticatorAttestationRawResponse AuthenticatorAttestation { get; init; }
}