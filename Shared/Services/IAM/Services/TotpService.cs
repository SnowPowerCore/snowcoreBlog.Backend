using OtpNet;

namespace snowcoreBlog.Backend.IAM.Services;

/// <summary>
/// RFC 6238 TOTP helper (30-second step, 6 digits, SHA-1 — the parameters used
/// by Microsoft Authenticator / Google Authenticator) on top of Otp.NET.
/// </summary>
public sealed class TotpService
{
    public const int SecretSizeBytes = 20;
    public const string Issuer = "snowcoreBlog";

    /// <summary>Generates a new Base32-encoded shared secret.</summary>
    public string GenerateSecret() =>
        Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(SecretSizeBytes));

    /// <summary>Builds the standard otpauth:// provisioning URI for QR codes.</summary>
    public string BuildOtpAuthUri(string secretBase32, string email) =>
        $"otpauth://totp/{Uri.EscapeDataString(Issuer)}:{Uri.EscapeDataString(email)}" +
        $"?secret={secretBase32}&issuer={Uri.EscapeDataString(Issuer)}&digits=6&period=30";

    /// <summary>
    /// Verifies a 6-digit code against the secret, accepting the previous and
    /// next time-step to tolerate clock drift.
    /// </summary>
    public bool Verify(string secretBase32, string code) =>
        new Totp(Base32Encoding.ToBytes(secretBase32))
            .VerifyTotp(code, out _, new VerificationWindow(previous: 1, future: 1));
}
