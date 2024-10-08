using FluentValidation;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.Push.Extensions;

namespace snowcoreBlog.Backend.Push.Validation;

public class CreateUserValidator : AbstractValidator<CreateUser>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.PhoneNumber).PhoneShared();
        RuleFor(x => x.ConfirmedAgreement).Equal(true);
    }
}