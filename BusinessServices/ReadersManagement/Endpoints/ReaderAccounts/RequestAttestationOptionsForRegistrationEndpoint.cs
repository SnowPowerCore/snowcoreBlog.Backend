using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Steps.Attestation;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.ReaderAccounts;

public class RequestAttestationOptionsForRegistrationEndpoint : Endpoint<RequestAttestationOptionsForRegistrationDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(RequestNewAttestationOptionsStep)
    ])]
    protected RequestAttestationOptionsDelegate RequestReaderAccountAttestationOptions { get; }

    public override void Configure()
    {
        Post("create/attestation");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Validator<RequestAttestationOptionsForRegistrationValidation>();
        AllowAnonymous();
    }

    [ServiceProviderSupplier]
    public RequestAttestationOptionsForRegistrationEndpoint(IServiceProvider _) { }

    public override async Task HandleAsync(RequestAttestationOptionsForRegistrationDto req, CancellationToken ct)
    {
        var context = new RequestAttestationOptionsContext(requestAttestationOptionsForRegistration: req);

        var result = await RequestReaderAccountAttestationOptions(context, ct);

        await SendAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}