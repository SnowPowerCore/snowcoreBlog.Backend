using snowcoreBlog.Backend.Push.Models;

namespace snowcoreBlog.Backend.Push.Options;

/// <summary>
/// Specifies options for password hashing.
/// </summary>
public record Argon2PasswordHasherOptions
{
    public Argon2HashStrength Strength { get; set; } = Argon2HashStrength.Moderate;
}