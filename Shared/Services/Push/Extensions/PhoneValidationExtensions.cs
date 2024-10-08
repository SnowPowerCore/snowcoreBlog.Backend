using FluentValidation;

namespace snowcoreBlog.Backend.Push.Extensions;

public static class PhoneValidationExtensions
{
    public static IRuleBuilderOptions<T, string> PhoneShared<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.NotEmpty().WithMessage("{PropertyName} is required.")
            .MinimumLength(10).WithMessage("{PropertyName} must not be less than 10 characters.")
            .MaximumLength(20).WithMessage("{PropertyName} must not exceed 20 characters.")
            .Matches(@"((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}").WithMessage("{PropertyName} is not valid");
}