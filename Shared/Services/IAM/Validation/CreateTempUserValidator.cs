using FluentValidation;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.PublicApi.Extensions;

namespace snowcoreBlog.Backend.IAM.Validation;

public class CreateTempUserValidator : AbstractValidator<CreateTempUser>
{
    public CreateTempUserValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().Length(3, 30);
        RuleFor(x => x.Email).EmailAddress().MinimumLength(3);
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.PhoneNumber)!.PhoneShared().When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
        RuleFor(x => x.ConfirmedAgreement).Equal(true);
        RuleFor(x => x.InitialEmailConsent).Equal(true);
    }
}