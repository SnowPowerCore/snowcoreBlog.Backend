using System.Net;
using System.Net.Mime;
using FastEndpoints;
using MassTransit;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Stripe.Interfaces.Services;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.Stripe.Endpoints.Webhook;

/// <summary>
/// The single Stripe webhook receiver. Security and reliability rules:
/// - the raw body MUST be read unmodified (no model binding) because the
///   Stripe-Signature HMAC is computed over the exact bytes Stripe sent;
/// - signature verification happens FIRST — unverified payloads never touch the DB;
/// - processed event ids are recorded in a unique-index ledger → exactly-once
///   handling even though Stripe retries with at-least-once delivery;
/// - unknown/unhandled event types are acknowledged with 200 (never 4xx — Stripe
///   disables endpoints that fail; only signature/config errors fail loudly).
/// </summary>
public class StripeWebhookEndpoint : EndpointWithoutRequest<string>
{
    private const string StripeSignatureHeader = "Stripe-Signature";

    public required IStripeWebhookProcessor WebhookProcessor { get; set; }

    public required IPublishEndpoint PublishEndpoint { get; set; }

    public override void Configure()
    {
        Post("webhook");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.Stripe)
            .Accepts<string>(MediaTypeNames.Application.Json)
            .Produces<string>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<string>((int)HttpStatusCode.BadRequest, MediaTypeNames.Application.Json)
            .Produces<string>((int)HttpStatusCode.InternalServerError, MediaTypeNames.Application.Json));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // Read the raw payload EXACTLY as received — required for HMAC verification.
        using var reader = new StreamReader(HttpContext.Request.Body);
        var json = await reader.ReadToEndAsync(ct);
        var signature = HttpContext.Request.Headers[StripeSignatureHeader].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(signature))
        {
            await Send.ResponseAsync("Missing payload or Stripe-Signature header.", (int)HttpStatusCode.BadRequest, ct);
            return;
        }

        var result = await WebhookProcessor.ProcessAsync(json, signature, ct);

        switch (result.Outcome)
        {
            case StripeWebhookOutcome.Succeeded:
                // Publish only after the state change was committed (at-least-once on
                // the bus; consumers tolerate replays via the record ids).
                foreach (var domainEvent in result.EventsToPublish)
                {
                    await PublishEndpoint.Publish(domainEvent, ct);
                }

                await Send.ResponseAsync("processed", (int)HttpStatusCode.OK, ct);
                return;

            case StripeWebhookOutcome.Duplicate:
                await Send.ResponseAsync("duplicate", (int)HttpStatusCode.OK, ct);
                return;

            case StripeWebhookOutcome.NotApplicable:
                await Send.ResponseAsync("ignored", (int)HttpStatusCode.OK, ct);
                return;

            case StripeWebhookOutcome.EntityNotFound:
                // Never 4xx on missing local state — Stripe would keep retrying and
                // eventually disable the endpoint. The warning is on our side.
                Logger.LogWarning("Stripe webhook could not find a matching local record: {Detail}", result.Detail);
                await Send.ResponseAsync("no matching record", (int)HttpStatusCode.OK, ct);
                return;

            case StripeWebhookOutcome.InvalidSignature:
                await Send.ResponseAsync("invalid signature", (int)HttpStatusCode.BadRequest, ct);
                return;

            case StripeWebhookOutcome.Misconfigured:
            default:
                await Send.ResponseAsync(result.Detail ?? "webhook misconfigured", (int)HttpStatusCode.InternalServerError, ct);
                return;
        }
    }
}
