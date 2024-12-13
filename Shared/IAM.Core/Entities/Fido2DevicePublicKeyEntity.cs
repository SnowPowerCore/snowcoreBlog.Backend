using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.IAM.Core.Entities;

public record Fido2DevicePublicKeyEntity : BaseEntity
{
    public required Guid? PublicKeyCredentialId { get; init; }

    public required byte[] Value { get; init; }
}