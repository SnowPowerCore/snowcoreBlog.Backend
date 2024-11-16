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

public class CreateReaderAccountEndpoint : Endpoint<CreateReaderAccountDto, ApiResponse?>
{
    private readonly JsonOptions _jsonOptions;

    [StepifiedProcess(Steps = [
        typeof(ValidateReaderAccountNotExistsStep),
        typeof(CreateUserForReaderAccountStep),
        typeof(CreateNewReaderEntityStep),
        typeof(SendEmailToNewReaderAccountStep),
        typeof(ReturnCreatedReaderEntityStep),
    ])]
    protected CreateReaderAccountDelegate CreateReaderAccount { get; }

    public override void Configure()
    {
        Post("readers/create");
        SerializerContext(CoreSerializationContext.Default);
        Validator<CreateReaderAccountValidation>();
        AllowAnonymous();
    }

    [ServiceProviderSupplier]
    public CreateReaderAccountEndpoint(IServiceProvider _,
                                       IOptions<JsonOptions> jsonOptions)
    {
        _jsonOptions = jsonOptions.Value;
    }

    public override async Task HandleAsync(CreateReaderAccountDto req, CancellationToken ct)
    {
        var context = new CreateReaderAccountContext(req);

        var result = await CreateReaderAccount(context, ct);

        await SendAsync(
            result?.ToApiResponse(serializerOptions: _jsonOptions.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}