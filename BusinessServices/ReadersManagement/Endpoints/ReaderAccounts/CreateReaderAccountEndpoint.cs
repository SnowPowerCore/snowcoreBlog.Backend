using System.Net;
using FastEndpoints;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.ReaderAccounts;

public class CreateReaderAccountEndpoint : Endpoint<CreateReaderAccountDto, ApiResponse?>
{
    [StepifiedProcess(Steps = [
        typeof(ValidateCreateReaderAccountStep),
        typeof(ValidateReaderAccountNotExistsStep),
        typeof(CreateUserForReaderAccountStep),
        typeof(CreateNewReaderEntityStep),
        typeof(SendEmailToNewReaderAccountStep),
        typeof(GenerateTokenForNewReaderAccountStep),
        typeof(ReturnCreatedReaderEntityStep),
    ])]
    protected CreateReaderAccountDelegate CreateReaderAccount { get; }

    public override void Configure()
    {
        Post("/readers/create");
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
    }

    [ServiceProviderSupplier]
    public CreateReaderAccountEndpoint(IServiceProvider _) { }

    public override async Task HandleAsync(CreateReaderAccountDto req, CancellationToken ct)
    {
        var context = new CreateReaderAccountContext(req);

        var result = await CreateReaderAccount(context, ct);

        await SendAsync(
            result?.ToApiResponse(),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}