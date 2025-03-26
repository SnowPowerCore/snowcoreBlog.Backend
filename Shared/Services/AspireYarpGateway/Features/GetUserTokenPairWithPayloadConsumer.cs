using System.Security.Claims;
using FastEndpoints.Security;
using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Options;
using MaybeResults;
using snowcoreBlog.Backend.AspireYarpGateway.Constants;
using snowcoreBlog.Backend.AspireYarpGateway.Options;
using snowcoreBlog.Backend.YarpGateway.Core.Contracts;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.AspireYarpGateway.Features;

/// <summary>
/// It is assumed that this consumer can only be called after all the authentication/authorization
/// checks have successfully passed in an invoker microservice and we're ready to serve a new token.
/// <para/>
/// This consumer doesn't do any additional business logic checks but executes a simple request data
/// validation.
/// </summary>
public class GetUserTokenPairWithPayloadConsumer : IConsumer<GetUserTokenPairWithPayload>
{
    private readonly IValidator<GetUserTokenPairWithPayload> _validator;
    private readonly SecuritySigningOptions _currentSecuritySigningOptions;

    public GetUserTokenPairWithPayloadConsumer(IValidator<GetUserTokenPairWithPayload> validator,
                                               IOptions<SecuritySigningOptions> securitySigningOpts)
    {
        _validator = validator;
        _currentSecuritySigningOptions = securitySigningOpts.Value;
    }

    public async Task Consume(ConsumeContext<GetUserTokenPairWithPayload> context)
    {
        var request = context.Message;
        var result = await _validator.ValidateAsync(request, context.CancellationToken);
        if (!result.IsValid)
        {
            await context.RespondAsync(
                new DataResult<UserTokenPairWithPayloadGenerated>(
                    Errors: result.Errors.Select(e => new NoneDetail(e.PropertyName, e.ErrorMessage)).ToList()));
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var accessTokenExpiresAt = now.AddMinutes(request.AccessTokenValidityDurationInMinutes).DateTime;
        var refreshTokenExpiresAt = now.AddMinutes(request.RefreshTokenValidityDurationInMinutes).DateTime;

        await context.RespondAsync(new DataResult<UserTokenPairWithPayloadGenerated>(new()
        {
            AccessToken = JwtBearer.CreateToken(jwt =>
            {
                jwt.Issuer = SecurityConstants.UserIssuer;
                jwt.SigningKey = _currentSecuritySigningOptions.User.SigningKey;
                jwt.SigningStyle = _currentSecuritySigningOptions.User.SigningStyle;
                jwt.SigningAlgorithm = _currentSecuritySigningOptions.User.SigningAlgorithm;
                jwt.KeyIsPemEncoded = _currentSecuritySigningOptions.User.KeyIsPemEncoded;
                jwt.ExpireAt = accessTokenExpiresAt;
                jwt.User.Permissions.AddRange(request.Permissions);
                jwt.User.Roles.AddRange(request.Roles);
                jwt.User.Claims.AddRange(request.Claims.Select(x => new Claim(x.Key, x.Value)));
            }),
            AccessTokenExpiresAt = accessTokenExpiresAt,
            RefreshToken = Guid.NewGuid().ToString("N"),
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        }));
    }
}