using FluentValidation;
using snowcoreBlog.Backend.Push.Core.Contracts;

namespace snowcoreBlog.Backend.Push.Validation;

public class GenericPushValidator : AbstractValidator<SendGenericPush>
{
    public GenericPushValidator()
    {
        RuleFor(x => x.Topic).NotEmpty();
        RuleFor(x => x.Subject).NotEmpty();
        RuleFor(x => x.Priority).NotNull();
    }
}