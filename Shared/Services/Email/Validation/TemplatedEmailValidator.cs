using FluentValidation;
using snowcoreBlog.Backend.Email.Core.Contracts;

namespace snowcoreBlog.Backend.Email.Validation;

public class TemplatedEmailValidator : AbstractValidator<SendTemplatedEmail>
{
    public TemplatedEmailValidator()
    {
        RuleFor(x => x.SenderAddress).EmailAddress();
        RuleFor(x => x.ReceiverAddress).EmailAddress();
        RuleFor(x => x.TemplateId).NotEmpty();
    }
}