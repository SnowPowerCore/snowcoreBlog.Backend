using FluentAssertions;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Validation;

namespace snowcoreBlog.Backend.IAM.Tests.Validation;

public class CreateUserValidatorTests
{
    private readonly CreateUserValidator _validator;

    public CreateUserValidatorTests()
    {
        _validator = new CreateUserValidator();
    }

    [Fact]
    public void Validate_WithValidEmail_ShouldPass()
    {
        // Arrange
        var createUser = new CreateUser
        {
            Email = "valid@example.com",
            TempUserVerificationToken = "valid-token",
            AttestationOptionsJson = "{}",
            AuthenticatorAttestation = new Fido2NetLib.AuthenticatorAttestationRawResponse
            {
                Id = new byte[] { 1, 2, 3 },
                RawId = new byte[] { 1, 2, 3 },
                Type = Fido2NetLib.Objects.PublicKeyCredentialType.PublicKey,
                Response = new Fido2NetLib.AuthenticatorAttestationRawResponse.ResponseData
                {
                    AttestationObject = new byte[] { 1, 2, 3 },
                    ClientDataJson = new byte[] { 1, 2, 3 }
                }
            }
        };

        // Act
        var result = _validator.Validate(createUser);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    public void Validate_WithInvalidEmail_ShouldFail(string email)
    {
        // Arrange
        var createUser = new CreateUser
        {
            Email = email,
            TempUserVerificationToken = "valid-token",
            AttestationOptionsJson = "{}",
            AuthenticatorAttestation = new Fido2NetLib.AuthenticatorAttestationRawResponse
            {
                Id = new byte[] { 1, 2, 3 },
                RawId = new byte[] { 1, 2, 3 },
                Type = Fido2NetLib.Objects.PublicKeyCredentialType.PublicKey,
                Response = new Fido2NetLib.AuthenticatorAttestationRawResponse.ResponseData
                {
                    AttestationObject = new byte[] { 1, 2, 3 },
                    ClientDataJson = new byte[] { 1, 2, 3 }
                }
            }
        };

        // Act
        var result = _validator.Validate(createUser);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_WithEmptyTempUserVerificationToken_ShouldFail()
    {
        // Arrange
        var createUser = new CreateUser
        {
            Email = "valid@example.com",
            TempUserVerificationToken = "",
            AttestationOptionsJson = "{}",
            AuthenticatorAttestation = new Fido2NetLib.AuthenticatorAttestationRawResponse
            {
                Id = new byte[] { 1, 2, 3 },
                RawId = new byte[] { 1, 2, 3 },
                Type = Fido2NetLib.Objects.PublicKeyCredentialType.PublicKey,
                Response = new Fido2NetLib.AuthenticatorAttestationRawResponse.ResponseData
                {
                    AttestationObject = new byte[] { 1, 2, 3 },
                    ClientDataJson = new byte[] { 1, 2, 3 }
                }
            }
        };

        // Act
        var result = _validator.Validate(createUser);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TempUserVerificationToken");
    }

    [Fact]
    public void Validate_WithNullAuthenticatorAttestation_ShouldFail()
    {
        // Arrange
        var createUser = new CreateUser
        {
            Email = "valid@example.com",
            TempUserVerificationToken = "valid-token",
            AttestationOptionsJson = "{}",
            AuthenticatorAttestation = null!
        };

        // Act
        var result = _validator.Validate(createUser);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AuthenticatorAttestation");
    }
}
