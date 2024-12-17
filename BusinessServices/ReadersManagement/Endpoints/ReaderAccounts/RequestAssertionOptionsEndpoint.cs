﻿using System.Net;
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

public class RequestAssertionOptionsEndpoint : Endpoint<RequestAssertionOptionsDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(RequestNewAssertionOptionsStep)
    ])]
    protected RequestAssertionOptionsDelegate RequestReaderAccountAssertionOptions { get; }

    public override void Configure()
    {
        Post("request/assertion");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Validator<RequestAssertionOptionsValidation>();
        AllowAnonymous();
    }

    [ServiceProviderSupplier]
    public RequestAssertionOptionsEndpoint(IServiceProvider _) { }

    public override async Task HandleAsync(RequestAssertionOptionsDto req, CancellationToken ct)
    {
        var context = new RequestAssertionOptionsContext(requestAssertionOptions: req);

        var result = await RequestReaderAccountAssertionOptions(context, ct);

        await SendAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}