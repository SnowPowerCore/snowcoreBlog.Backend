using Fido2NetLib;
using Fido2NetLib.Objects;

namespace snowcoreBlog.Backend.IAM.Extensions;

public static class Fido2Extensions
{
    public static RequestNewCredentialParams ToRequestNewCredentialParams(
        this Fido2User user, AttestationConveyancePreference attestationConveyancePreference,
        AuthenticatorSelection authenticatorSelection, AuthenticationExtensionsClientInputs authenticationExtensionsClientInputs) =>
        new()
        {
            User = user,
            AttestationPreference = attestationConveyancePreference,
            AuthenticatorSelection = authenticatorSelection,
            Extensions = authenticationExtensionsClientInputs
        };

    public static GetAssertionOptionsParams ToGetAssertionOptionsParams(
        this List<PublicKeyCredentialDescriptor> allowedCredentials,
        UserVerificationRequirement userVerification,
        AuthenticationExtensionsClientInputs extensions) =>
        new()
        {
            AllowedCredentials = allowedCredentials,
            UserVerification = userVerification,
            Extensions = extensions
        };

    public static MakeNewCredentialParams ToMakeNewCredentialParams(
        AuthenticatorAttestationRawResponse authenticatorAttestation,
        CredentialCreateOptions credentialCreateOptions,
        IsCredentialIdUniqueToUserAsyncDelegate isCredentialIdUniqueToUserAsync) =>
        new()
        {
            IsCredentialIdUniqueToUserCallback = isCredentialIdUniqueToUserAsync,
            AttestationResponse = authenticatorAttestation,
            OriginalOptions = credentialCreateOptions
        };
}