using FluentValidation;

namespace snowcoreBlog.Backend.Push.Extensions;

public static class PasswordValidationExtensions
{
    public static IRuleBuilderOptions<T, string> PasswordShared<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.NotEmpty().WithMessage("{PropertyName} cannot be empty")
            .MinimumLength(8).WithMessage("{PropertyName} length must be at least 8.")
            .MaximumLength(16).WithMessage("{PropertyName} length must not exceed 16.")
            .Matches(@"[A-Z]+").WithMessage("{PropertyName} must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("{PropertyName} must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("{PropertyName} must contain at least one number.")
            .Matches(@"[\#\!\?\*\.]+").WithMessage("{PropertyName} must contain at least one (#!?*.).");
}