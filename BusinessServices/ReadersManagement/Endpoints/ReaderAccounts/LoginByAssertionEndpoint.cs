using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Steps.Assertion;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.ReaderAccounts;

public class LoginByAssertionEndpoint : Endpoint<LoginByAssertionDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ValidateReaderAccountExistsStep),
        typeof(AttemptLoginByAssertionStep),
    ])]
    protected LoginByAssertionDelegate LoginByAssertion { get; }

    public override void Configure()
    {
        Post("login/assertion");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Validator<LoginByAssertionValidation>();
        AllowAnonymous();
    }

    [ServiceProviderSupplier]
    public LoginByAssertionEndpoint(IServiceProvider _) { }

    public override async Task HandleAsync(LoginByAssertionDto req, CancellationToken ct)
    {
        var context = new LoginByAssertionContext(req);

        var result = await LoginByAssertion(context, ct);

        await SendAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}