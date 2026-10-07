using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Mime;
using FastEndpoints;
using MaybeResults;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.ReaderAccounts;

public class RequestAuthDataEndpoint(
    JsonWebTokenHandler securityTokenHandler,
    TokenValidationParameters validationParameters) : EndpointWithoutRequest<ApiResponse>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    public override void Configure()
    {
        Get("check/me");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        EnableAntiforgery();
        Claims(ReaderAccountClaimConstants.ReaderAccountReaderAccountClaimKey);
        Description(b => b
            .WithTags(ApiTagConstants.ReaderAccountManagement)
            .Produces<ApiResponseForOpenApi<AuthenticationStateDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.InternalServerError, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
            // Read the first Authorization value only. A multi-valued header (e.g. when both
            // the YARP gateway and this service's UserCookieJsonWebTokenMiddleware add a value)
            // would otherwise join as "Bearer jwt,Bearer jwt" and corrupt the token.
            var authorizationValues = HttpContext.Request.Headers[HeaderNames.Authorization];
            var authorizationHeader = authorizationValues.Count == 0
                ? string.Empty
                : authorizationValues[0];
            if (string.IsNullOrWhiteSpace(authorizationHeader) ||
                !authorizationHeader.StartsWith($"{JwtBearerDefaults.AuthenticationScheme} ", StringComparison.OrdinalIgnoreCase))
            {
                await Send.ResponseAsync(
                    ErrorResponseUtilities.ApiResponseWithErrors(["Missing or invalid authorization header"], (int)HttpStatusCode.Unauthorized),
                    (int)HttpStatusCode.Unauthorized,
                    ct);
                return;
            }

            var accessToken = authorizationHeader[$"{JwtBearerDefaults.AuthenticationScheme} ".Length..].Trim();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                await Send.ResponseAsync(
                    ErrorResponseUtilities.ApiResponseWithErrors(["Missing access token"], (int)HttpStatusCode.Unauthorized),
                    (int)HttpStatusCode.Unauthorized,
                    ct);
                return;
            }

            // Validate signature + lifetime AND read claims in one step, using the same
            // signing key configuration that JwtTokenCreator uses to mint the token.
            // This returns a precise exception (SecurityTokenMalformed / signature / lifetime)
            // rather than a bare boolean from CanReadToken.
            var validationResult = await securityTokenHandler.ValidateTokenAsync(accessToken, validationParameters);
            if (!validationResult.IsValid)
            {
                await Send.ResponseAsync(
                    ErrorResponseUtilities.ApiResponseWithErrors(
                        [$"Access token validation failed: {validationResult.Exception?.Message}"],
                        (int)HttpStatusCode.Unauthorized),
                    (int)HttpStatusCode.Unauthorized,
                    ct);
                return;
            }

            // Claims come from the validated ClaimsIdentity (signature + lifetime already verified).
            var claims = validationResult.ClaimsIdentity?.Claims
                         ?? (validationResult.SecurityToken as JsonWebToken)?.Claims
                         ?? [];
            await Send.ResponseAsync(
                Maybe.Create(new AuthenticationStateDto(claims.ToDictionary(x => x.Type, x => x.Value)))
                    .ToApiResponse(JsonOptions.Value.SerializerOptions), (int)HttpStatusCode.OK, ct);
            return;
        }
        catch (Exception)
        {
            await Send.ResponseAsync(
                ReaderAccountCouldNotReadDataError<AuthenticationStateDto>.Create(ReaderAccountUserConstants.RequestAuthDataUnableToGetData)
                    .ToApiResponse(JsonOptions.Value.SerializerOptions), cancellation: ct);
            return;
        }
    }
}