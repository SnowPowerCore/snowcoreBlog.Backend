using System.Text.Json.Serialization;

namespace snowcoreBlog.Backend.Email.Core.Models.Email;

public sealed record ActivateCreatedTempUserData
{
    [JsonPropertyName("subject")]
    public string? Subject { get; init; }

    [JsonPropertyName("preHeader")]
    public string? PreHeader { get; init; }

    [JsonPropertyName("userFirstName")]
    public string? UserFirstName { get; init; }

    [JsonPropertyName("verificationUrl")]
    public string? VerificationUrl { get; init; }

    [JsonPropertyName("verificationTokenUntilThatDate")]
    public string? VerificationTokenUntilThatDate { get; init; }
}