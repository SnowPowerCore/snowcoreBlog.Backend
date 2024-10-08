using FluentValidation;
using snowcoreBlog.Backend.Email.Core.Contracts;

namespace snowcoreBlog.Backend.Email.Validation;

public class GenericEmailValidator : AbstractValidator<SendGenericEmail>
{
    public GenericEmailValidator()
    {
        RuleFor(x => x.SenderAddress).EmailAddress();
        RuleFor(x => x.NotifiedEntityAddress).EmailAddress();
        RuleFor(x => x.Subject).NotEmpty();
    }
}