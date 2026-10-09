using FluentValidation;
using snowcoreBlog.Backend.IAM.Core.Contracts;

namespace snowcoreBlog.Backend.IAM.Validation;

public class ValidateUserOneTimeCodeValidator : AbstractValidator<ValidateUserOneTimeCode>
{
    public ValidateUserOneTimeCodeValidator()
    {
        RuleFor(x => x.Email).EmailAddress().MinimumLength(3);
        RuleFor(x => x.Code).NotEmpty().Matches(@"^\d{6}$");
    }
}
