using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.ReaderAccounts;

public class RequestCreateReaderAccountEndpoint : Endpoint<RequestCreateReaderAccountDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ValidateReaderAccountEmailDomainStep),
        typeof(ValidateReaderAccountNotExistsStep),
        typeof(ValidateReaderAccountTempRecordNotExistsStep),
        typeof(ValidateReaderAccountNickNameWasNotTaken),
        typeof(CreateReaderAccountTempUserStep),
    ])]
    protected RequestCreateReaderAccountDelegate RequestCreateReaderAccount { get; }

    public override void Configure()
    {
        Post("readers/create/request");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Validator<RequestCreateReaderAccountValidation>();
        AllowAnonymous();
    }

    [ServiceProviderSupplier]
    public RequestCreateReaderAccountEndpoint(IServiceProvider _) { }

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