using snowcoreBlog.Backend.Core.Base;

namespace snowcoreBlog.Backend.IAM.Core.Entities;

public record Fido2PublicKeyCredentialEntity : BaseEntity
{
    public required IList<byte> PublicKeyCredentialId { get; init; }

    public required IList<byte> PublicKey { get; init; }

    public required uint SignatureCounter { get; init; }

    public bool IsBackupEligible { get; init; }

    public bool IsBackedUp { get; init; }

    public required IList<byte> AttestationObject { get; init; }

    public required IList<byte> AttestationClientDataJson { get; init; }

    public required string AttestationFormat { get; init; }

    public required Guid AaGuid { get; init; }

    public required Guid UserId { get; init; }

    public ICollection<Fido2AuthenticatorTransportEntity> AuthenticatorTransports { get; init; } = [];

    public ICollection<Fido2DevicePublicKeyEntity> DevicePublicKeys { get; init; } = [];
}