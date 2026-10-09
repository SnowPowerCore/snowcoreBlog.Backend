using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Authenticator;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.ReaderAccounts;

public class SetupAuthenticatorEndpoint : Endpoint<SetupReaderAccountAuthenticatorDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ValidateReaderAccountTempRecordExistsStep),
        typeof(SetupAuthenticatorForTempUserStep),
    ])]
    protected SetupAuthenticatorDelegate SetupReaderAccountAuthenticator { get; }

    public override void Configure()
    {
        Post("create/totp/setup");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Validator<SetupReaderAccountAuthenticatorValidator>();
        AllowAnonymous();
        EnableAntiforgery();
        Description(b => b
            .WithTags(ApiTagConstants.ReaderAccountManagement)
            .Accepts<SetupReaderAccountAuthenticatorDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<SetupReaderAccountAuthenticatorResultDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.InternalServerError, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(SetupReaderAccountAuthenticatorDto req, CancellationToken ct)
    {
        var context = new SetupAuthenticatorContext(req);

        var result = await SetupReaderAccountAuthenticator(context, ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
