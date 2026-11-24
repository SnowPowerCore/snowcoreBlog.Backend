using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Stripe.Context;
using snowcoreBlog.Backend.Stripe.Delegates;
using snowcoreBlog.Backend.Stripe.Steps.Donations;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.Stripe.Endpoints.Donations;

/// <summary>
/// Creates a Stripe Checkout Session for a one-time donation and returns the hosted
/// Checkout URL. The donation itself is confirmed asynchronously by Stripe webhooks
/// (POST webhook) which flip the record to Succeeded and publish <c>DonationPaid</c>.
/// The whole business flow is a StepifiedProcess — insert extra steps (fraud
/// screening, recurring upsell, ...) into the <c>Steps</c> array below.
/// </summary>
public class CreateDonationCheckoutEndpoint : Endpoint<CreateDonationCheckoutDto, ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ValidateCreateDonationCheckoutStep),
        typeof(CreateDonationCheckoutSessionStep),
        typeof(PersistDonationStep),
        typeof(BuildDonationCheckoutResultStep),
    ])]
    protected CreateDonationCheckoutDelegate CreateDonationCheckout { get; } = default!;

    public override void Configure()
    {
        Post("donations/checkout");
        Version(1);
        SerializerContext(StripeSerializationContext.Default);
        Validator<CreateDonationCheckoutValidator>();
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.Stripe)
            .Accepts<CreateDonationCheckoutDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<DonationCheckoutResultDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.InternalServerError, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CreateDonationCheckoutDto req, CancellationToken ct)
    {
        var context = new CreateDonationCheckoutContext(req);
        var result = await CreateDonationCheckout(context, ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
