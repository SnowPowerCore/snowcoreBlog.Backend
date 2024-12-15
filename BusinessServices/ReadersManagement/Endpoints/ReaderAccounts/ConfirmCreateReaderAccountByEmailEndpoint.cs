using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Confirm;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.ReaderAccounts;

public class ConfirmCreateReaderAccountByEmailEndpoint : Endpoint<ConfirmCreateReaderAccountDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ValidateReaderAccountNotExistsStep),
        typeof(CreateReaderAccountUserStep),
        typeof(CreateReaderEntityForNewUserStep),
        typeof(ReturnCreatedReaderEntityStep),
    ])]
    protected ConfirmCreateReaderAccountDelegate ConfirmCreateReaderAccount { get; }

    public override void Configure()
    {
        Post("create/confirm/email");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Validator<ConfirmCreateReaderAccountValidation>();
        AllowAnonymous();
    }

    [ServiceProviderSupplier]
    public ConfirmCreateReaderAccountByEmailEndpoint(IServiceProvider _) { }

    public override async Task HandleAsync(ConfirmCreateReaderAccountDto req, CancellationToken ct)
    {
        var context = new ConfirmCreateReaderAccountContext(req);

        var result = await ConfirmCreateReaderAccount(context, ct);

        await SendAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}