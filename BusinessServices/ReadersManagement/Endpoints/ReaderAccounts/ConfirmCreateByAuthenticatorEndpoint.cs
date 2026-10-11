using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.StepProgress;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Authenticator;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.ReaderAccounts;

public class ConfirmCreateByAuthenticatorEndpoint : Endpoint<ConfirmCreateReaderAccountWithAuthenticatorDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ValidateReaderAccountNotExistsForAuthenticatorStep),
        typeof(CreateReaderAccountUserWithAuthenticatorStep),
        typeof(CreateReaderEntityForAuthenticatorStep),
        typeof(ReturnCreatedReaderEntityForAuthenticatorStep),
    ])]
    protected ConfirmCreateReaderAccountWithAuthenticatorDelegate ConfirmCreateReaderAccountWithAuthenticator { get; }

    public override void Configure()
    {
        Post("create/confirm/totp");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Validator<ConfirmCreateReaderAccountWithAuthenticatorValidator>();
        AllowAnonymous();
        EnableAntiforgery();
        Description(b => b
            .WithTags(ApiTagConstants.ReaderAccountManagement)
            .Accepts<ConfirmCreateReaderAccountWithAuthenticatorDto>(MediaTypeNames.Application.Json)
            .Produces((int)HttpStatusCode.OK, contentType: MediaTypeNames.Text.EventStream)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(ConfirmCreateReaderAccountWithAuthenticatorDto req, CancellationToken ct)
    {
        await HttpContext.SendStepProgressStreamAsync(
            async (reporter, token) =>
            {
                var context = new ConfirmCreateReaderAccountWithAuthenticatorContext(req);
                context.AttachStepProgressReporter(reporter);

                var result = await ConfirmCreateReaderAccountWithAuthenticator(context, token);

                return (result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
                    result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError);
            },
            ct);
    }
}
