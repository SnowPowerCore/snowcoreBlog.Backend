using System.Net;
using FastEndpoints;
using MinimalStepifiedSystem.Attributes;
using Results;
using snowcoreBlog.PublicApi;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ReadersManagement;

public class CreateReaderAccountEndpoint : Endpoint<CreateReaderAccountDto, ApiResponse?>
{
    [StepifiedProcess(Steps = [
        typeof(ValidateCreateReaderAccountStep),
        typeof(ValidateReaderAccountNotExistsStep),
        typeof(CreateUserForReaderAccountStep),
        typeof(CreateNewReaderEntityStep),
        typeof(SendEmailToNewReaderAccountStep),
    ])]
    protected CreateReaderAccountDelegate CreateReaderAccount { get; }

    public override void Configure()
    {
        Post("/readers/create");
        AllowAnonymous();
    }

    [ServiceProviderSupplier]
    public CreateReaderAccountEndpoint(IServiceProvider _) { }

    public override async Task HandleAsync(CreateReaderAccountDto req, CancellationToken ct)
    {
        var context = new CreateReaderAccountContext(req);

        await CreateReaderAccount(context, ct);
        var result = context.GetFromData<IResult<ReaderAccountCreationResultDto>>(
            ReaderAccountConstants.CreateReaderAccountResult);

        await SendAsync(
            result?.ToApiResponse(),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}