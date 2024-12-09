using FluentValidation;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.Email.Core.Models.Email;

namespace snowcoreBlog.Backend.Email.Validation;

public class ActivateCreatedTempUserTemplatedEmailValidator : AbstractValidator<SendTemplatedEmail<ActivateCreatedTempUserData>>
{
    public ActivateCreatedTempUserTemplatedEmailValidator()
    {
        RuleFor(x => x.SenderAddress).EmailAddress();
        RuleFor(x => x.ReceiverAddress).EmailAddress();
        RuleFor(x => x.TemplateId).NotEmpty();
    }
}