using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.IAM.Core.Entities;

public record Fido2DevicePublicKeyEntity : BaseEntity
{
    public required Guid PublicKeyId { get; init; }

    public required IList<Byte> PublicKeyCredentialId { get; init; }

    public required IList<Byte> Value { get; init; }
}