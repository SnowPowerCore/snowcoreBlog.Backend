```instructions
This is the Backend Stripe microservice — donation acceptance + payments for
goods/services via Stripe Checkout Sessions, webhook-driven state and MassTransit
events. It is intentionally complete on the processing side: consuming services only
need to call the checkout endpoints / listen to the events and build their UI.

Big picture:
- Two checkout flows (donation, goods) share one shape: StepifiedProcess pipeline
  (Validate → CreateSession → Persist → BuildResult). The local record is persisted
  in `Pending` BEFORE the hosted Checkout URL is returned — webhooks racing the HTTP
  response always find their record.
- `Endpoints/Webhook/StripeWebhookEndpoint.cs` is the single Stripe webhook receiver:
  raw-body read (HMAC over exact bytes), signature verification FIRST, exactly-once
  via `StripeWebhookEventEntity` (unique EventId index), guarded transitions only
  (Pending/Processing → terminal), unknown events acknowledged with 200.
- `Services/StripeWebhookProcessor.cs` owns all webhook logic; it parses the raw
  `data.object` JSON (Stripe's stable public API) instead of Stripe.net typed event
  models — deliberate decoupling from SDK churn.
- Records carry `client_reference_id` + metadata (`payment_record_id`,
  `payment_record_kind`) so every webhook payload resolves back to the local entity.

Key wiring (read `Program.cs`):
- Npgsql `db-stripe-entities`, Marten with 4 document types + unique indexes
  (checkout session ids, webhook event ids, stripe customer ids) +
  `CoreSerializationContext.Default` serializer.
- MassTransit/RabbitMQ — `EnsureStripeCustomerConsumer` (request/response contract
  `EnsureStripeCustomer` → `DataResult<StripeCustomerEnsured>`).
- Stripe.net (`IStripeClient` + typed services) registered lazily in
  `StripeClientConfigurationExtensions` — the host starts fine WITHOUT credentials;
  only checkout endpoints demand `Stripe:SecretKey`, only the webhook demands
  `Stripe:WebhookSecret` (500 without it, by design).
- Options: `Stripe` section → `Options/StripeOptions.cs` (currency allowlist, amount
  bounds, session lifetime, success/cancel URL templates).

Conventions here (mirror Documents/ReadersManagement):
- Steps live in `Steps/Donations/` + `Steps/Payments/`, registered in
  `Extensions/Startup/CoreServicesConfigurationExtensions.cs`; keep in sync with the
  endpoints' `[StepifiedProcess(Steps = [...])]` arrays.
- Errors: `[None]` records `DonationError`/`PaymentError` → `DonationError<T>.Create(...)`.
- Entity↔DTO mapping via Mapperly (`Extensions/StripeMapper.cs`).
- Response envelope: `IMaybe<T>.ToApiResponse()` + `ToStatusCode()`.

Cross-service touchpoints when changing DTOs/events:
- Entities/enums: `Shared/Core/Entities/Stripe/**`; contracts: `Shared/Core/Contracts/Stripe/**`.
- DTOs/validators/tag: backend + frontend `snowcoreBlog.PublicApi` copies
  (`ApiTagConstants.Stripe`); register all new types in
  `snowcoreBlog.Backend.Infrastructure/Context/CoreSerializationContext.cs`.
- Gateway routes: Aspire host `appsettings.Development.json` (`/api/stripe/**`,
  cluster `backend-stripe`).

Reliability invariants (do not regress):
- No model binding on the webhook endpoint; raw body only.
- One Postgres transaction per webhook (entity update + ledger row share the scoped
  Marten session; single `SaveChangesAsync`).
- Domain events published only AFTER the webhook transaction commits.
- Endpoint-level status changes are impossible — webhooks are the only writer.

Run/build:
- Aspire host (recommended): `dotnet run --project ./snowcoreBlog.Aspire.csproj`.
- Standalone: `dotnet run --project ./snowcoreBlog.Backend/Shared/Services/Stripe/snowcoreBlog.Backend.Stripe.csproj`.
- Local webhook testing: `stripe listen --forward-to https://localhost/api/stripe/webhook`.

Known simplifications: anonymous endpoints (auth policies when the real consumer is
wired), one-time payments only (no subscriptions yet), donation refunds have no event
contract yet, no Redis.
```
