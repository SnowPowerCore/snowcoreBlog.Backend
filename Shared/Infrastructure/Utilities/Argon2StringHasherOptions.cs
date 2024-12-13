namespace snowcoreBlog.Backend.Infrastructure.Utilities;

/// <summary>
/// Specifies options for password hashing.
/// </summary>
public class Argon2StringHasherOptions
{
    public Argon2HashStrength Strength { get; set; } = Argon2HashStrength.Moderate;
}