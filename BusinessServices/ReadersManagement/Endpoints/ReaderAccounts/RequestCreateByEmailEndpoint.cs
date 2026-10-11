using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.StepProgress;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
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
        typeof(ValidateReaderAccountNotExistsStep),
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
        Validator<RequestCreateReaderAccountValidator>();
        AllowAnonymous();
        EnableAntiforgery();
        Tags(EndpointTagConstants.RequireCaptchaVerification);
        Description(b => b
            .WithTags(ApiTagConstants.ReaderAccountManagement, ApiTagConstants.Captcha)
            .Accepts<RequestCreateReaderAccountDto>(MediaTypeNames.Application.Json)
            .Produces((int)HttpStatusCode.OK, contentType: MediaTypeNames.Text.EventStream)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(RequestCreateReaderAccountDto req, CancellationToken ct)
    {
        await HttpContext.SendStepProgressStreamAsync(
            async (reporter, token) =>
            {
                var context = new RequestCreateReaderAccountContext(req);
                context.AttachStepProgressReporter(reporter);

                var result = await RequestCreateReaderAccount(context, token);

                return (result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
                    result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError);
            },
            ct);
    }
}