using System.Net;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Steps.NickName;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.ReaderAccounts;

public class CheckNickNameNotTakenEndpoint : Endpoint<CheckNickNameNotTakenDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ValidateNickNameWasNotTakenStep),
    ])]
    protected CheckNickNameNotTakenDelegate CheckNickNameNotTaken { get; }

    public override void Configure()
    {
        Post("check/nickname");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Validator<CheckNickNameNotTakenValidation>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CheckNickNameNotTakenDto req, CancellationToken ct)
    {
        var context = new CheckNickNameNotTakenContext(req.NickName);

        var result = await CheckNickNameNotTaken(context, ct);

        await SendAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}