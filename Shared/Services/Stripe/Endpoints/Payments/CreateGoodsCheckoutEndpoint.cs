using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Stripe.Context;
using snowcoreBlog.Backend.Stripe.Delegates;
using snowcoreBlog.Backend.Stripe.Steps.Payments;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.Stripe.Endpoints.Payments;

/// <summary>
/// Creates a Stripe Checkout Session for goods/services and returns the hosted
/// Checkout URL. <see cref="CreateGoodsCheckoutDto.ReferenceId"/> correlates the payment
/// with the caller's order; fulfilment happens downstream on the <c>PaymentPaid</c>
/// event. Insert extra steps (inventory reservation, tax calc, ...) below.
/// </summary>
public class CreateGoodsCheckoutEndpoint : Endpoint<CreateGoodsCheckoutDto, ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ValidateCreateGoodsCheckoutStep),
        typeof(CreateGoodsCheckoutSessionStep),
        typeof(PersistPaymentStep),
        typeof(BuildGoodsCheckoutResultStep),
    ])]
    protected CreateGoodsCheckoutDelegate CreateGoodsCheckout { get; } = default!;

    public override void Configure()
    {
        Post("payments/checkout");
        Version(1);
        SerializerContext(StripeSerializationContext.Default);
        Validator<CreateGoodsCheckoutValidator>();
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.Stripe)
            .Accepts<CreateGoodsCheckoutDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<GoodsCheckoutResultDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.InternalServerError, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CreateGoodsCheckoutDto req, CancellationToken ct)
    {
        var context = new CreateGoodsCheckoutContext(req);
        var result = await CreateGoodsCheckout(context, ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
