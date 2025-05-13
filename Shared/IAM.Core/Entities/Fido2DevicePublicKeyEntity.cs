using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.IAM.Core.Entities;

public record Fido2DevicePublicKeyEntity : BaseEntity
{
    public required Guid PublicKeyId { get; init; }

    public required string PublicKeyCredentialId { get; init; }

    public required string Value { get; init; }
}