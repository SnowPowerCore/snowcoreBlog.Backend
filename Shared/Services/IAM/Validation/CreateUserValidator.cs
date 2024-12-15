using FluentValidation;
using snowcoreBlog.Backend.IAM.Core.Contracts;

namespace snowcoreBlog.Backend.IAM.Validation;

public class CreateUserValidator : AbstractValidator<CreateUser>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email).EmailAddress().MinimumLength(3);
        RuleFor(x => x.TempUserVerificationToken).NotEmpty();
        RuleFor(x => x.AuthenticatorAttestation).NotNull();
    }
}