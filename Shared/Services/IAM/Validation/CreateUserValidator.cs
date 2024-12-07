using FluentValidation;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Extensions;

namespace snowcoreBlog.Backend.IAM.Validation;

public class CreateUserValidator : AbstractValidator<CreateUser>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email).EmailAddress().MinimumLength(3);
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.PhoneNumber).PhoneShared();
        RuleFor(x => x.ConfirmedAgreement).Equal(true);
    }
}