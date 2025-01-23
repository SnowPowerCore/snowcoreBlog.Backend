using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Request;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.ReaderAccounts;

public class RequestCreateByEmailEndpoint : Endpoint<RequestCreateReaderAccountDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ValidateReaderAccountEmailDomainStep),
        typeof(ValidateReaderAccountNotExistStep),
        typeof(ValidateReaderAccountTempRecordNotExistsStep),
        typeof(ValidateReaderAccountNickNameWasNotTakenStep),
        typeof(CreateReaderAccountTempUserStep),
    ])]
    protected RequestCreateReaderAccountDelegate RequestCreateReaderAccount { get; }

    public override void Configure()
    {
        Post("create/request/email");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Validator<RequestCreateReaderAccountValidation>();
        AllowAnonymous();
        Tags(EnpointTagConstants.RequireCaptchaVerification);
        Description(b => b
            .WithTags(ApiTagConstants.ReaderAccountManagement)
            .Accepts<RequestCreateReaderAccountDto>("application/json")
            .Produces<ApiResponseForOpenApi<RequestReaderAccountCreationResultDto>>((int)HttpStatusCode.OK, "application/json")
            .Produces<ApiResponse>((int)HttpStatusCode.InternalServerError, "application/json")
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(RequestCreateReaderAccountDto req, CancellationToken ct)
    {
        var context = new RequestCreateReaderAccountContext(req);

        var result = await RequestCreateReaderAccount(context, ct);

        await SendAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}