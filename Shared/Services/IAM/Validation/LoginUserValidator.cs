using FluentValidation;
using snowcoreBlog.Backend.IAM.Core.Contracts;

namespace snowcoreBlog.Backend.IAM.Validation;

public class LoginUserValidator : AbstractValidator<LoginUser>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email).EmailAddress().MinimumLength(3);
        RuleFor(x => x.AssertionOptionsJson).NotEmpty();
        RuleFor(x => x.AuthenticatorAssertion).NotNull();
    }
}