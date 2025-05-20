using Fido2NetLib;
using Fido2NetLib.Objects;
using Riok.Mapperly.Abstractions;

namespace snowcoreBlog.Backend.IAM.Extensions;

[Mapper]
public static partial class Fido2Extensions
{
    [MapPropertyFromSource(nameof(RequestNewCredentialParams.User))]
    public static partial RequestNewCredentialParams ToRequestNewCredentialParams(
        this Fido2User user, AttestationConveyancePreference AttestationPreference,
        AuthenticatorSelection authenticatorSelection, AuthenticationExtensionsClientInputs extensions);

    [MapPropertyFromSource(nameof(GetAssertionOptionsParams.AllowedCredentials))]
    public static partial GetAssertionOptionsParams ToGetAssertionOptionsParams(
        this IReadOnlyList<PublicKeyCredentialDescriptor> allowedCredentials,
        UserVerificationRequirement userVerification,
        AuthenticationExtensionsClientInputs extensions);

    [MapPropertyFromSource(nameof(MakeNewCredentialParams.AttestationResponse))]
    public static partial MakeNewCredentialParams ToMakeNewCredentialParams(
        this AuthenticatorAttestationRawResponse attestationResponse,
        CredentialCreateOptions originalOptions,
        IsCredentialIdUniqueToUserAsyncDelegate IsCredentialIdUniqueToUserCallback);

    [MapPropertyFromSource(nameof(MakeAssertionParams.AssertionResponse))]
    public static partial MakeAssertionParams ToMakeAssertionParams(
        this AuthenticatorAssertionRawResponse assertionResponse,
        AssertionOptions originalOptions,
        byte[] storedPublicKey,
        uint storedSignatureCounter,
        IsUserHandleOwnerOfCredentialIdAsync isUserHandleOwnerOfCredentialIdCallback);
}