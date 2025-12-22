using System.IdentityModel.Tokens.Jwt;
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

public class RequestAuthDataEndpoint(JwtSecurityTokenHandler securityTokenHandler) : EndpointWithoutRequest<ApiResponse>
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

    public override Task HandleAsync(CancellationToken ct)
    {
        try
        {
            var authorizationHeader = HttpContext.Request.Headers[HeaderNames.Authorization].ToString();
            if (string.IsNullOrWhiteSpace(authorizationHeader) ||
                !authorizationHeader.StartsWith($"{JwtBearerDefaults.AuthenticationScheme} ", StringComparison.OrdinalIgnoreCase))
            {
                return Send.ResponseAsync(
                    ErrorResponseUtilities.ApiResponseWithErrors(["Missing or invalid authorization header"], (int)HttpStatusCode.Unauthorized),
                    (int)HttpStatusCode.Unauthorized,
                    ct);
            }

            var accessToken = authorizationHeader[$"{JwtBearerDefaults.AuthenticationScheme} ".Length..].Trim();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return Send.ResponseAsync(
                    ErrorResponseUtilities.ApiResponseWithErrors(["Missing access token"], (int)HttpStatusCode.Unauthorized),
                    (int)HttpStatusCode.Unauthorized,
                    ct);
            }

            var decodedToken = securityTokenHandler.ReadJwtToken(accessToken);
            return Send.ResponseAsync(
                Maybe.Create(new AuthenticationStateDto(decodedToken.Claims.ToDictionary(x => x.Type, x => x.Value)))
                    .ToApiResponse(JsonOptions.Value.SerializerOptions), (int)HttpStatusCode.OK, ct);
        }
        catch (Exception)
        {
            return Send.ResponseAsync(
                ReaderAccountCouldNotReadDataError<AuthenticationStateDto>.Create(ReaderAccountUserConstants.RequestAuthDataUnableToGetData)
                    .ToApiResponse(JsonOptions.Value.SerializerOptions), cancellation: ct);
        }
    }
}