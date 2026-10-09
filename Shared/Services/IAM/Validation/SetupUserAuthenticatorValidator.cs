using FluentValidation;
using snowcoreBlog.Backend.IAM.Core.Contracts;

namespace snowcoreBlog.Backend.IAM.Validation;

public class SetupUserAuthenticatorValidator : AbstractValidator<SetupUserAuthenticator>
{
    public SetupUserAuthenticatorValidator()
    {
        RuleFor(x => x.Email).EmailAddress().MinimumLength(3);
        RuleFor(x => x.TempUserVerificationToken).NotEmpty();
    }
}
