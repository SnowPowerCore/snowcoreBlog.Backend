using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Steps.OneTimeCode;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.ReaderAccounts;

public class VerifyOneTimeCodeEndpoint : Endpoint<VerifyOneTimeCodeDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ResolveReaderAccountForOneTimeCodeStep),
        typeof(VerifyAuthenticatorOneTimeCodeStep),
        typeof(IssueTokenForOneTimeCodeLoginStep),
    ])]
    protected LoginByOneTimeCodeDelegate LoginByOneTimeCode { get; }

    public override void Configure()
    {
        Post("login/totp/verify");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Validator<VerifyOneTimeCodeValidator>();
        AllowAnonymous();
        EnableAntiforgery();
        Tags(EndpointTagConstants.RequireCaptchaVerification);
        Description(b => b
            .WithTags(ApiTagConstants.ReaderAccountManagement, ApiTagConstants.Captcha)
            .Accepts<VerifyOneTimeCodeDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<LoginByOneTimeCodeResultDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.InternalServerError, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(VerifyOneTimeCodeDto req, CancellationToken ct)
    {
        var context = new LoginByOneTimeCodeContext(req);

        var result = await LoginByOneTimeCode(context, ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
