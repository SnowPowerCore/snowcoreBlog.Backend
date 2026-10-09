using FluentValidation;
using snowcoreBlog.Backend.IAM.Core.Contracts;

namespace snowcoreBlog.Backend.IAM.Validation;

public class CreateUserWithAuthenticatorValidator : AbstractValidator<CreateUserWithAuthenticator>
{
    public CreateUserWithAuthenticatorValidator()
    {
        RuleFor(x => x.Email).EmailAddress().MinimumLength(3);
        RuleFor(x => x.TempUserVerificationToken).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().Matches(@"^\d{6}$");
    }
}
