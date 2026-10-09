namespace snowcoreBlog.Backend.IAM.Core.Contracts;

/// <summary>
/// Provisioning payload for an authenticator app: the Base32 shared secret
/// and the otpauth:// URI to encode as a QR code.
/// </summary>
public sealed record AuthenticatorSetupResult
{
    public required string Secret { get; init; }

    public required string OtpAuthUri { get; init; }
}
