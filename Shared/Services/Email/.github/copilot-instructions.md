```instructions
This is the Backend Email microservice (messaging worker — no business HTTP endpoints).

What it does:
- Consumes email commands from RabbitMQ and sends via SendGrid or Amazon SES: `Features/SendGrid/*` (generic/templated), `Features/AmazonSimpleEmail/*` (generic/templated), plus disposable-email domain checks (`Features/Validation/CheckEmailDomainConsumer` backed by an Apizr external API).

Key wiring (read `Program.cs` and `Extensions/Startup/*`):
- MassTransit consumers registered in `MassTransitConfigurationExtensions`; RabbitMQ connection string `rabbitmq`.
- Redis for domain-check caching; `ExternalApiConfigurationExtensions` wires the disposable-email API via Apizr.
- Service contracts: `Api/IEmailDisposableApi`, `Api/IStaticEmailDisposableApi`.
- Service defaults via `TelemetryConfigurationExtensions` in Program.

Patterns:
- Senders/queries are extension helpers in `Extensions/AmazonSimpleEmailExtensions.cs` / `SendGridExtensions.cs`.
- FluentValidation validators in `Validation/`; consumers short-circuit on invalid messages.

When editing:
- Sender identities come from each producer's `Integrations:Email:SenderAccount` options (ReadersManagement/AdminsManagement pass them in the message) — do not hardcode sender addresses here.
- Message contracts live in `snowcoreBlog.Universal/PublicApi` (duplicated into frontend/console copies — update all three).
```
