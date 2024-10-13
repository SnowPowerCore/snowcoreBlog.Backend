using FluentValidation;
using snowcoreBlog.Backend.Email.Core.Contracts;

namespace snowcoreBlog.Backend.Email.Validation;

public class CheckEmailDomainValidator : AbstractValidator<CheckEmailDomain>
{
    public CheckEmailDomainValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
    }
}