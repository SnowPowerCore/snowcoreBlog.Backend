# Stripe Microservice (snowcoreBlog.Backend.Stripe)

Payment microservice for the snowcoreBlog platform. Provides **donation acceptance**
and **payments for goods/services** through Stripe Checkout Sessions, with webhook-driven
state transitions, exactly-once event processing and MassTransit events for downstream
consumers. Built to be wired up from the outside (UI + consuming services) without any
further changes inside the microservice.

## What it does

- `POST v1/donations/checkout` — creates a Stripe Checkout Session (payment mode,
  `donate` submit type) for a one-time donation; returns the hosted Checkout URL.
- `POST v1/payments/checkout` — creates a Checkout Session for goods/services; the
  caller-supplied `referenceId` correlates the payment with the consumer's order.
- `GET v1/donations/{id}` / `GET v1/payments/{id}` — status snapshots for polling.
- `POST v1/webhook` — the single Stripe webhook receiver (signature-verified,
  exactly-once via an event-id ledger, raw-body HMAC).
- `EnsureStripeCustomer` MassTransit request/response — other services ensure a Stripe
  Customer exists for their accounts (`StripeCustomerEnsured` reply).

## Payment flow (both flows are identical in shape)

1. Client calls a checkout endpoint → record is persisted in `Pending` **before** the
   Checkout URL is returned (webhooks racing the HTTP response always find it).
2. Client redirects to `checkoutUrl` (Stripe-hosted page).
3. Stripe calls the webhook → signature verified → transition applied atomically
   (entity update + ledger row in one Postgres transaction) → `DonationPaid` /
   `PaymentPaid` / `PaymentFailed` / `PaymentRefunded` published on RabbitMQ.
4. Consuming service fulfils on the event (or polls the GET endpoints).

## Key wiring (read `Program.cs`)

- Npgsql data source `db-stripe-entities` (database `snowcore_blog_stripe`), Marten with
  `StripeCustomerEntity`, `DonationEntity`, `PaymentEntity`, `StripeWebhookEventEntity`
  (+ unique indexes on checkout-session ids, Stripe event ids and customer ids) and the
  `CoreSerializationContext.Default` serializer.
- MassTransit/RabbitMQ with the `EnsureStripeCustomerConsumer` (request/response).
- FastEndpoints + JWT bearer (signing key `Security:Signing:User:SigningKey`), Scalar
  OpenAPI in Development (`/openapi/v1.json`).
- Options bound from the `Stripe` section (`Options/StripeOptions.cs`): `SecretKey`,
  `WebhookSecret`, currency allowlist, donation/goods amount bounds, session lifetime,
  success/cancel URL templates.

## Secrets (never in source control)

- `Stripe:SecretKey` (`sk_test_...` / `sk_live_...`) — user secrets or environment.
- `Stripe:WebhookSecret` (`whsec_...`) — from the Stripe Dashboard webhook endpoint;
  the service refuses webhook processing (HTTP 500) without it, by design.

Local Stripe testing: `stripe listen --forward-to https://localhost/api/stripe/webhook`
and `stripe trigger checkout.session.completed`.

## StepifiedProcess conventions

- Donation: `Context/CreateDonationCheckoutContext.cs`, `Delegates/CreateDonationCheckoutDelegate.cs`,
  steps in `Steps/Donations/` (Validate → CreateSession → Persist → BuildResult).
- Goods: same shape under `Steps/Payments/`.
- Steps are DI-registered in `Extensions/Startup/CoreServicesConfigurationExtensions.cs`;
  keep that list in sync with the endpoints' `[StepifiedProcess(Steps = [...])]` arrays.
- Errors: `[None]` records `ErrorResults/DonationError.cs` / `PaymentError.cs`.

## Webhook rules (do not break these)

- The endpoint reads the **raw body** — the signature HMAC is computed over exact bytes;
  never add model binding there.
- Transitions are guarded (only `Pending`/`Processing` → terminal states); replays of an
  already-handled event id return 200 without re-applying.
- Unknown event types and "entity not found" are acknowledged with 200 — only signature
  failures (400) and misconfiguration (500) fail loudly, since Stripe disables endpoints
  that answer with errors.

## Cross-service touchpoints when changing DTOs/events

- Entities/enums: `snowcoreBlog.Backend/Shared/Core/Entities/Stripe/**`.
- Contracts: `Shared/Core/Contracts/Stripe/**` (`EnsureStripeCustomer`,
  `StripeCustomerEnsured`, `DonationPaid`, `DonationFailed`, `PaymentPaid`,
  `PaymentFailed`, `PaymentRefunded`).
- DTOs: `snowcoreBlog.PublicApi` (`CreateDonationCheckoutDto`, `DonationCheckoutResultDto`,
  `CreateGoodsCheckoutDto`, `GoodsCheckoutResultDto`, `DonationDto`, `PaymentDto`,
  `StripePaymentStatusDto`), tag `ApiTagConstants.Stripe`; mapping via Mapperly
  (`Extensions/StripeMapper.cs`).
- Register new serializable types in
  `snowcoreBlog.Backend.Infrastructure/Context/CoreSerializationContext.cs` and keep the
  3 PublicApi copies (Backend/Frontend/Console) in sync when a consumer needs them
  (Backend + Frontend copies already carry the Stripe DTOs).
- Gateway routes live in the Aspire host `appsettings.Development.json` (`/api/stripe/**`).

## Run/build

- Aspire host (recommended): `dotnet run --project ./snowcoreBlog.Aspire.csproj`.
- Standalone: `dotnet run --project ./snowcoreBlog.Backend/Shared/Services/Stripe/snowcoreBlog.Backend.Stripe.csproj`.

## Known simplifications (decided, revisit when needed)

- Endpoints are `AllowAnonymous` for now; auth policies (e.g. `regularReader` + owner
  checks on GET endpoints) are expected when the real consumer is wired through the gateway.
- Only one-time payments (Checkout `payment` mode); subscriptions / recurring donations
  would reuse the same pipeline with a `subscription`-mode step.
- Donation refunds flip the record status but carry no dedicated event contract yet.
- No Redis/cache — nothing here is hot enough to warrant it yet.
