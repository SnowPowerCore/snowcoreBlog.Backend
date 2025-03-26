using FluentValidation;
using snowcoreBlog.Backend.YarpGateway.Core.Contracts;

namespace snowcoreBlog.Backend.AspireYarpGateway.Validation;

public class GetUserTokenPairWithPayloadValidator : AbstractValidator<GetUserTokenPairWithPayload>
{
    public GetUserTokenPairWithPayloadValidator()
    {
        RuleFor(x => x.Roles).NotNull();
        RuleFor(x => x.Claims).NotNull();
        RuleFor(x => x.AccessTokenValidityDurationInMinutes).NotEmpty();
    }
}