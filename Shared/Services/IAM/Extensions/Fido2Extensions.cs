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
}